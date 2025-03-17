using ClosedXML.Excel;
using System.Text;
using WDBXlsxTool.Support;

namespace WDBXlsxTool.XIII2LR.Conversion
{
    internal class XlsxDeserializer
    {
        private static int SheetIndex = 1;

        public static void DeserializeData(string inXlsxFile, WDBVariablesXIII2LR wdbVars)
        {
            Console.WriteLine("Loading workbook....");
            Console.WriteLine("");

            SheetIndex = 1;

            using (var wdbWorkbook = new XLWorkbook(inXlsxFile))
            {
                Console.WriteLine("Deserializing main sections....");
                Console.WriteLine("");
                DeserializeMainSections(wdbWorkbook, wdbVars);

                Console.WriteLine("Deserializing records....");
                Console.WriteLine("");
                DeserializeRecords(wdbWorkbook, wdbVars);
            }
        }


        private static void DeserializeMainSections(XLWorkbook wdbWorkbook, WDBVariablesXIII2LR wdbVars)
        {
            XlsxHelpers.CheckIfSheetExists(wdbWorkbook, "!!info");
            XlsxHelpers.CheckIfSheetExists(wdbWorkbook, wdbVars.StructItemSectionName);
            XlsxHelpers.CheckIfSheetExists(wdbWorkbook, wdbVars.VersionSectionName);

            IXLWorksheet currentSheet;

            currentSheet = wdbWorkbook.Worksheet("!!info");
            SheetIndex++;

            // Get all info
            wdbVars.HasSheetName = Convert.ToBoolean(currentSheet.Cell(1, 2).Value.ToString());
            wdbVars.HasStrArraySection = Convert.ToBoolean(currentSheet.Cell(2, 2).Value.ToString());
            wdbVars.HasTypelistSection = Convert.ToBoolean(currentSheet.Cell(3, 2).Value.ToString());
            wdbVars.ParseStrtypelistAsV1 = Convert.ToBoolean(currentSheet.Cell(4, 2).Value.ToString());

            if (wdbVars.HasStrArraySection)
            {
                // Get strArrayInfo values
                XlsxHelpers.CheckIfSheetExists(wdbWorkbook, wdbVars.StrArrayInfoSectionName);

                currentSheet = wdbWorkbook.Worksheet(wdbVars.StrArrayInfoSectionName);
                SheetIndex++;
                wdbVars.RecordCountWithSections += 3;

                wdbVars.BitsPerOffset = Convert.ToByte(currentSheet.Cell(1, 2).Value.ToString());
                wdbVars.OffsetsPerValue = Convert.ToByte(currentSheet.Cell(2, 2).Value.ToString());

                wdbVars.StrArrayInfoData = new byte[4];
                wdbVars.StrArrayInfoData[2] = wdbVars.OffsetsPerValue;
                wdbVars.StrArrayInfoData[3] = wdbVars.BitsPerOffset;
            }

            // Get strtypelist values
            if (wdbVars.ParseStrtypelistAsV1)
            {
                XlsxHelpers.CheckIfSheetExists(wdbWorkbook, wdbVars.StrtypelistSectionName);
                currentSheet = wdbWorkbook.Worksheet(wdbVars.StrtypelistSectionName);

                wdbVars.StrtypelistValues = XlsxHelpers.GetValuesForListSection(currentSheet);

                wdbVars.StrtypelistData = new byte[wdbVars.StrtypelistValues.Count * 4];
                wdbVars.StrtypelistData = SharedMethods.CreateArrayFromUIntList(wdbVars.StrtypelistValues);
            }
            else
            {
                XlsxHelpers.CheckIfSheetExists(wdbWorkbook, wdbVars.StrtypelistbSectionName);
                currentSheet = wdbWorkbook.Worksheet(wdbVars.StrtypelistbSectionName);

                wdbVars.StrtypelistValues = XlsxHelpers.GetValuesForListSection(currentSheet);

                wdbVars.StrtypelistData = new byte[wdbVars.StrtypelistValues.Count];

                for (int i = 0; i < wdbVars.StrtypelistValues.Count; i++)
                {
                    wdbVars.StrtypelistData[i] = (byte)wdbVars.StrtypelistValues[i];
                }
            }

            SheetIndex++;
            wdbVars.RecordCountWithSections++;

            // Get typelist values if present
            if (wdbVars.HasTypelistSection)
            {
                XlsxHelpers.CheckIfSheetExists(wdbWorkbook, wdbVars.TypelistSectionName);
                currentSheet = wdbWorkbook.Worksheet(wdbVars.TypelistSectionName);
                SheetIndex++;
                wdbVars.RecordCountWithSections++;

                wdbVars.TypelistValues = XlsxHelpers.GetValuesForListSection(currentSheet);

                wdbVars.TypelistData = new byte[wdbVars.TypelistValues.Count * 4];
                wdbVars.TypelistData = SharedMethods.CreateArrayFromUIntList(wdbVars.TypelistValues);
            }

            // Get version value
            currentSheet = wdbWorkbook.Worksheet(wdbVars.VersionSectionName);
            SheetIndex++;
            wdbVars.RecordCountWithSections++;

            wdbVars.VersionData = BitConverter.GetBytes(Convert.ToUInt32(currentSheet.Cell(1, 1).Value.ToString()));
            Array.Reverse(wdbVars.VersionData);


            // Get structitem values
            currentSheet = wdbWorkbook.Worksheet(wdbVars.StructItemSectionName);
            SheetIndex++;
            wdbVars.RecordCountWithSections += 2;

            wdbVars.Fields = XlsxHelpers.GetFieldsFromSheet(currentSheet);
            wdbVars.FieldCount = (uint)wdbVars.Fields.Length;

            var structItemsList = new List<byte>();

            for (int i = 0; i < wdbVars.FieldCount; i++)
            {
                structItemsList.AddRange(Encoding.UTF8.GetBytes(wdbVars.Fields[i] + "\0"));
            }

            wdbVars.StructItemData = structItemsList.ToArray();
            wdbVars.StructItemNumData = BitConverter.GetBytes(wdbVars.FieldCount);
            Array.Reverse(wdbVars.StructItemNumData);

            // Get sheetName
            if (wdbVars.HasSheetName)
            {
                wdbVars.RecordCountWithSections++;

                wdbVars.SheetName = wdbWorkbook.Worksheet(SheetIndex).Name;

                wdbVars.SheetName += "\0";
                wdbVars.SheetNameData = Encoding.UTF8.GetBytes(wdbVars.SheetName);
            }

            // Determine whether there is
            // a string section
            if (!wdbVars.HasStringSection)
            {
                if (wdbVars.StrtypelistValues.Contains(2))
                {
                    wdbVars.HasStringSection = true;
                    wdbVars.RecordCountWithSections++;
                }
                else if (wdbVars.HasStrArraySection)
                {
                    wdbVars.HasStringSection = true;
                    wdbVars.RecordCountWithSections++;
                }
            }
        }


        private static void DeserializeRecords(XLWorkbook wdbWorkbook, WDBVariablesXIII2LR wdbVars)
        {
            var currentSheet = wdbWorkbook.Worksheet(SheetIndex);

            var currentRow = 2;
            int currentColumn;

            string recordName;
            string fieldName;

            while (true)
            {
                currentColumn = 1;

                recordName = currentSheet.Cell(currentRow, currentColumn).Value.ToString();
                currentColumn++;

                if (string.IsNullOrEmpty(recordName))
                {
                    break;
                }

                wdbVars.RecordCount++;
                var currentDataList = new List<object>();

                // Get record data
                for (int f = 0; f < wdbVars.FieldCount; f++)
                {
                    fieldName = wdbVars.Fields[f];

                    if (fieldName.StartsWith("s"))
                    {
                        currentDataList.Add(currentSheet.Cell(currentRow, currentColumn).Value.ToString());
                    }
                    else
                    {
                        currentDataList.Add(Convert.ToDecimal(currentSheet.Cell(currentRow, currentColumn).Value.ToString()));
                    }

                    currentColumn++;
                }

                wdbVars.RecordsDataDict.Add(recordName, currentDataList);

                currentRow++;
            }

            wdbVars.RecordCountWithSections += wdbVars.RecordCount;
        }
    }
}