using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using WDBXlsxTool.Support;

namespace WDBXlsxTool.XIII.Conversion
{
    internal class XlsxDeserializer
    {
        private static int SheetIndex = 0;

        public static void DeserializeData(string inXlsxFile, WDBVariablesXIII wdbVars)
        {
            using (var wdbWorkbook = new XSSFWorkbook(inXlsxFile))
            {
                Console.WriteLine("Deserializing main sections....");
                Console.WriteLine("");
                DeserializeMainSections(wdbWorkbook, wdbVars);

                Console.WriteLine("Deserializing records....");
                Console.WriteLine("");
                DeserializeRecords(wdbWorkbook, wdbVars);
            }
        }


        private static void DeserializeMainSections(XSSFWorkbook wdbWorkbook, WDBVariablesXIII wdbVars)
        {
            ISheet currentSheet;
            IRow currentRow;

            // Get recordCount and determine
            // if the file is known
            currentSheet = wdbWorkbook.GetSheet("!!info");
            SheetIndex++;
            currentRow = currentSheet.GetRow(0);
            wdbVars.RecordCount = Convert.ToUInt32(currentRow.GetCell(1).ToString());

            currentRow = currentSheet.GetRow(1);
            wdbVars.IsKnown = Convert.ToBoolean(currentRow.GetCell(1).ToString());

            // Get strtypelist values
            currentSheet = wdbWorkbook.GetSheet(wdbVars.StrtypelistSectionName);
            SheetIndex++;
            wdbVars.StrtypelistValues = GetValuesFromListSection(currentSheet);

            if (!wdbVars.IsKnown)
            {
                wdbVars.FieldCount = (uint)wdbVars.StrtypelistValues.Count;
            }

            wdbVars.StrtypelistData = new byte[wdbVars.StrtypelistValues.Count * 4];
            wdbVars.StrtypelistData = SharedMethods.CreateArrayFromUIntList(wdbVars.StrtypelistValues);

            wdbVars.RecordCountWithSections++;

            // Get typelist values
            currentSheet = wdbWorkbook.GetSheet(wdbVars.TypelistSectionName);
            SheetIndex++;
            wdbVars.TypelistValues = GetValuesFromListSection(currentSheet);

            wdbVars.TypelistData = new byte[wdbVars.TypelistValues.Count * 4];
            wdbVars.TypelistData = SharedMethods.CreateArrayFromUIntList(wdbVars.TypelistValues);

            wdbVars.RecordCountWithSections++;

            // Get version value
            currentSheet = wdbWorkbook.GetSheet(wdbVars.VersionSectionName);
            SheetIndex++;
            currentRow = currentSheet.GetRow(0);
            wdbVars.VersionData = BitConverter.GetBytes(Convert.ToUInt32(currentRow.GetCell(0).ToString()));
            Array.Reverse(wdbVars.VersionData);

            wdbVars.RecordCountWithSections++;

            wdbVars.RecordCountWithSections += wdbVars.RecordCount;

            // Get structitem values
            // if the file is known
            if (wdbVars.IsKnown)
            {
                currentSheet = wdbWorkbook.GetSheet(wdbVars.StructItemSectionName);
                SheetIndex++;
                int rowIndex = 0;
                int cellIndex = 0;
                var structItemValues = new List<string>();

                while (true)
                {
                    currentRow = currentSheet.GetRow(rowIndex);

                    if (currentRow == null)
                    {
                        break;
                    }

                    structItemValues.Add(Convert.ToString(currentRow.GetCell(cellIndex)));

                    rowIndex++;
                }

                wdbVars.Fields = structItemValues.ToArray();
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

        private static List<uint> GetValuesFromListSection(ISheet currentSheet)
        {
            var listSectionValues = new List<uint>();
            IRow currentRow;
            int rowIndex = 0;
            int cellIndex = 0;

            while (true)
            {
                currentRow = currentSheet.GetRow(rowIndex);

                if (currentRow == null)
                {
                    break;
                }

                listSectionValues.Add(Convert.ToUInt32(currentRow.GetCell(cellIndex).ToString()));

                rowIndex++;
            }

            return listSectionValues;
        }


        public static void DeserializeRecords(XSSFWorkbook wdbWorkbook, WDBVariablesXIII wdbVars)
        {
            ISheet currentSheet;
            IRow currentRow;
            int rowIndex = 1;
            int cellIndex;

            currentSheet = wdbWorkbook.GetSheetAt(SheetIndex);

            var recordName = string.Empty;
            string fieldName;

            for (int i = 0; i < wdbVars.RecordCount; i++)
            {
                cellIndex = 0;

                currentRow = currentSheet.GetRow(rowIndex);
                recordName = Convert.ToString(currentRow.GetCell(cellIndex));
                cellIndex++;

                var currentDataList = new List<object>();

                // Get record data
                if (wdbVars.IsKnown)
                {
                    for (int f = 0; f < wdbVars.FieldCount; f++)
                    {
                        fieldName = wdbVars.Fields[f];

                        if (fieldName.StartsWith("s"))
                        {
                            currentDataList.Add(Convert.ToString(currentRow.GetCell(cellIndex)));
                        }
                        else
                        {
                            currentDataList.Add(Convert.ToDouble(currentRow.GetCell(cellIndex).ToString()));
                        }

                        cellIndex++;
                    }
                }
                else
                {
                    for (int f = 0; f < wdbVars.FieldCount; f++)
                    {
                        switch (wdbVars.StrtypelistValues[f])
                        {
                            case 0:
                                currentDataList.Add(Convert.ToString(currentRow.GetCell(cellIndex)));
                                break;

                            case 1:
                                currentDataList.Add(Convert.ToDouble(currentRow.GetCell(cellIndex).ToString()));
                                break;

                            case 2:
                                currentDataList.Add(Convert.ToString(currentRow.GetCell(cellIndex)));
                                break;

                            case 3:
                                currentDataList.Add(Convert.ToUInt32(currentRow.GetCell(cellIndex).ToString()));
                                break;
                        }

                        cellIndex++;
                    }
                }

                wdbVars.RecordsDataDict.Add(recordName, currentDataList);

                rowIndex++;
            }
        }
    }
}