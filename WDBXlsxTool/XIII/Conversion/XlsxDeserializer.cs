using ClosedXML.Excel;
using WDBXlsxTool.Support;

namespace WDBXlsxTool.XIII.Conversion
{
    internal class XlsxDeserializer
    {
        private static int SheetIndex = 1;

        public static void DeserializeData(string inXlsxFile, WDBVariablesXIII wdbVars)
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


        private static void DeserializeMainSections(XLWorkbook wdbWorkbook, WDBVariablesXIII wdbVars)
        {
            XlsxHelpers.CheckIfSheetExists(wdbWorkbook, "!!info");
            XlsxHelpers.CheckIfSheetExists(wdbWorkbook, wdbVars.StrtypelistSectionName);
            XlsxHelpers.CheckIfSheetExists(wdbWorkbook, wdbVars.TypelistSectionName);
            XlsxHelpers.CheckIfSheetExists(wdbWorkbook, wdbVars.VersionSectionName);

            IXLWorksheet currentSheet;

            // Determine if the file
            // is known
            currentSheet = wdbWorkbook.Worksheet("!!info");
            SheetIndex++;

            wdbVars.IsKnown = Convert.ToBoolean(currentSheet.Cell(1, 2).Value.ToString());

            // Get strtypelist values
            currentSheet = wdbWorkbook.Worksheet(wdbVars.StrtypelistSectionName);
            SheetIndex++;
            wdbVars.RecordCountWithSections++;

            wdbVars.StrtypelistValues = XlsxHelpers.GetValuesForListSection(currentSheet);

            if (!wdbVars.IsKnown)
            {
                wdbVars.FieldCount = (uint)wdbVars.StrtypelistValues.Count;
            }

            wdbVars.StrtypelistData = new byte[wdbVars.StrtypelistValues.Count * 4];
            wdbVars.StrtypelistData = SharedMethods.CreateArrayFromUIntList(wdbVars.StrtypelistValues);

            // Get typelist values
            currentSheet = wdbWorkbook.Worksheet(wdbVars.TypelistSectionName);
            SheetIndex++;
            wdbVars.RecordCountWithSections++;

            wdbVars.TypelistValues = XlsxHelpers.GetValuesForListSection(currentSheet);

            wdbVars.TypelistData = new byte[wdbVars.TypelistValues.Count * 4];
            wdbVars.TypelistData = SharedMethods.CreateArrayFromUIntList(wdbVars.TypelistValues);

            // Get version value
            currentSheet = wdbWorkbook.Worksheet(wdbVars.VersionSectionName);
            SheetIndex++;
            wdbVars.RecordCountWithSections++;

            wdbVars.VersionData = BitConverter.GetBytes(Convert.ToUInt32(currentSheet.Cell(1, 1).Value.ToString()));
            Array.Reverse(wdbVars.VersionData);

            // Get structitem values
            // if the file is known
            if (wdbVars.IsKnown)
            {
                XlsxHelpers.CheckIfSheetExists(wdbWorkbook, wdbVars.StructItemSectionName);

                currentSheet = wdbWorkbook.Worksheet(wdbVars.StructItemSectionName);
                SheetIndex++;

                wdbVars.Fields = XlsxHelpers.GetFieldsFromSheet(currentSheet);
                wdbVars.FieldCount = (uint)wdbVars.Fields.Length;
            }

            // Determine whether there is
            // a string section
            wdbVars.RecordCountWithSections++;

            if (!wdbVars.HasStringSection)
            {
                if (wdbVars.StrtypelistValues.Contains(2))
                {
                    wdbVars.HasStringSection = true;
                }
            }
        }


        private static void DeserializeRecords(XLWorkbook wdbWorkbook, WDBVariablesXIII wdbVars)
        {
            var currentSheet = wdbWorkbook.Worksheet(SheetIndex);
            wdbVars.SheetName = currentSheet.Name;

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
                if (wdbVars.IsKnown)
                {
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
                }
                else
                {
                    for (int f = 0; f < wdbVars.FieldCount; f++)
                    {
                        switch (wdbVars.StrtypelistValues[f])
                        {
                            case 0:
                                currentDataList.Add(currentSheet.Cell(currentRow, currentColumn).Value.ToString());
                                break;

                            case 1:
                                currentDataList.Add(currentSheet.Cell(currentRow, currentColumn).Value.ToString());
                                break;

                            case 2:
                                currentDataList.Add(currentSheet.Cell(currentRow, currentColumn).Value.ToString());
                                break;

                            case 3:
                                currentDataList.Add(Convert.ToUInt32(currentSheet.Cell(currentRow, currentColumn).Value.ToString()));
                                break;
                        }

                        currentColumn++;
                    }
                }

                wdbVars.RecordsDataDict.Add(recordName, currentDataList);

                currentRow++;
            }

            wdbVars.RecordCountWithSections += wdbVars.RecordCount;
        }
    }
}