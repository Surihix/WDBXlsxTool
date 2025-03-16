using ClosedXML.Excel;
using WDBXlsxTool.Support;

namespace WDBXlsxTool.XIII.Conversion
{
    internal class XlsxDeserializer
    {
        private static int SheetIndex = 1;

        public static void DeserializeData(string inXlsxFile, WDBVariablesXIII wdbVars)
        {
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
            CheckIfSheetExists(wdbWorkbook, "!!info");
            CheckIfSheetExists(wdbWorkbook, wdbVars.StrtypelistSectionName);
            CheckIfSheetExists(wdbWorkbook, wdbVars.TypelistSectionName);
            CheckIfSheetExists(wdbWorkbook, wdbVars.VersionSectionName);

            IXLWorksheet currentSheet;

            // Get recordCount and determine
            // if the file is known
            currentSheet = wdbWorkbook.Worksheet("!!info");
            SheetIndex++;
            wdbVars.RecordCount = Convert.ToUInt32(currentSheet.Cell(1, 2).Value.ToString());
            wdbVars.IsKnown = Convert.ToBoolean(currentSheet.Cell(2, 2).Value.ToString());

            // Get strtypelist values
            currentSheet = wdbWorkbook.Worksheet(wdbVars.StrtypelistSectionName);
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
            currentSheet = wdbWorkbook.Worksheet(wdbVars.TypelistSectionName);
            SheetIndex++;
            wdbVars.TypelistValues = GetValuesFromListSection(currentSheet);

            wdbVars.TypelistData = new byte[wdbVars.TypelistValues.Count * 4];
            wdbVars.TypelistData = SharedMethods.CreateArrayFromUIntList(wdbVars.TypelistValues);

            wdbVars.RecordCountWithSections++;

            // Get version value
            currentSheet = wdbWorkbook.Worksheet(wdbVars.VersionSectionName);
            SheetIndex++;
            wdbVars.VersionData = BitConverter.GetBytes(Convert.ToUInt32(currentSheet.Cell(1, 1).Value.ToString()));
            Array.Reverse(wdbVars.VersionData);

            wdbVars.RecordCountWithSections++;

            wdbVars.RecordCountWithSections += wdbVars.RecordCount;

            // Get structitem values
            // if the file is known
            if (wdbVars.IsKnown)
            {
                CheckIfSheetExists(wdbWorkbook, wdbVars.StructItemSectionName);

                currentSheet = wdbWorkbook.Worksheet(wdbVars.StructItemSectionName);
                SheetIndex++;

                string currentVal;
                var currentRow = 1;
                var structItemValues = new List<string>();

                while (true)
                {
                    currentVal = currentSheet.Cell(currentRow, 1).Value.ToString();

                    if (string.IsNullOrEmpty(currentVal))
                    {
                        break;
                    }

                    structItemValues.Add(currentVal.ToString());

                    currentRow++;
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

        private static void CheckIfSheetExists(XLWorkbook workbook, string sheetName)
        {
            if (workbook.TryGetWorksheet(sheetName, out _) == false)
            {
                SharedMethods.ErrorExit($"Missing {sheetName} in specified xlsx file");
            }
        }

        private static List<uint> GetValuesFromListSection(IXLWorksheet currentSheet)
        {
            string currentVal;
            var currentRow = 1;
            var listSectionValues = new List<uint>();

            while (true)
            {
                currentVal = currentSheet.Cell(currentRow, 1).Value.ToString();

                if (string.IsNullOrEmpty(currentVal))
                {
                    break;
                }

                listSectionValues.Add(Convert.ToUInt32(currentVal));

                currentRow++;
            }

            return listSectionValues;
        }


        private static void DeserializeRecords(XLWorkbook wdbWorkbook, WDBVariablesXIII wdbVars)
        {
            var currentSheet = wdbWorkbook.Worksheet(SheetIndex);

            var currentRow = 2;
            int currentColumn;

            string recordName;
            string fieldName;

            for (int i = 0; i < wdbVars.RecordCount; i++)
            {
                currentColumn = 1;

                recordName = currentSheet.Cell(currentRow, currentColumn).Value.ToString();
                currentColumn++;

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
        }
    }
}