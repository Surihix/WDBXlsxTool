using ClosedXML.Excel;

namespace WDBXlsxTool.Support
{
    internal class XlsxHelpers
    {
        public static void WriteToCell(IXLWorksheet xLWorksheet, int row, int column, WriteType writeType, object valueToWrite, bool bold)
        {
            switch (writeType)
            {
                case WriteType.Int:
                    xLWorksheet.Cell(row, column).Value = Convert.ToInt32(valueToWrite);
                    break;

                case WriteType.Float:
                    xLWorksheet.Cell(row, column).Value = valueToWrite.ToString();
                    break;

                case WriteType.String:
                    xLWorksheet.Cell(row, column).Value = valueToWrite.ToString();
                    break;

                case WriteType.UInt32:
                    xLWorksheet.Cell(row, column).Value = Convert.ToUInt32(valueToWrite);
                    break;

                case WriteType.UInt64:
                    xLWorksheet.Cell(row, column).Value = Convert.ToUInt64(valueToWrite);
                    break;

                case WriteType.Boolean:
                    xLWorksheet.Cell(row, column).Value = valueToWrite.ToString();
                    break;
            }

            xLWorksheet.Cell(row, column).Style.Font.Bold = bold;
            xLWorksheet.Cell(row, column).Style.Font.FontName = "Arial";
            xLWorksheet.Cell(row, column).Style.Font.FontSize = 12;
        }

        public enum WriteType
        {
            Int,
            Float,
            String,
            UInt32,
            UInt64,
            Boolean
        }


        public static void AutoAdjustRowsAndColumns(IXLWorksheet xLWorksheet)
        {
            xLWorksheet.Rows().AdjustToContents();
            xLWorksheet.Columns().AdjustToContents();
        }


        public static List<uint> WriteListSectionValues(byte[] listSectionData, IXLWorksheet currentSheet)
        {
            var listSectionValues = new List<uint>();

            var listIndex = 0;
            var currentlistData = new byte[4];
            var listValueCount = listSectionData.Length / 4;
            uint listValue;
            var currentRow = 1;

            for (int s = 0; s < listValueCount; s++)
            {
                Array.ConstrainedCopy(listSectionData, listIndex, currentlistData, 0, 4);
                Array.Reverse(currentlistData);
                listValue = BitConverter.ToUInt32(currentlistData, 0);

                listSectionValues.Add(listValue);

                WriteToCell(currentSheet, currentRow, 1, WriteType.UInt32, listValue, false);
                currentRow++;

                listIndex += 4;
            }

            return listSectionValues;
        }


        public static void WriteStructItemDataToSheet(IXLWorksheet xLWorksheet, uint fieldsCount, string[] fields)
        {
            var currentRow = 1;

            for (int i = 0; i < fieldsCount; i++)
            {
                WriteToCell(xLWorksheet, currentRow, 1, WriteType.String, fields[i], false);
                currentRow++;
            }
        }


        public static void CheckIfSheetExists(XLWorkbook workbook, string sheetName)
        {
            if (workbook.TryGetWorksheet(sheetName, out _) == false)
            {
                SharedMethods.ErrorExit($"Missing {sheetName} in specified xlsx file");
            }
        }


        public static List<uint> GetValuesForListSection(IXLWorksheet xLWorksheet)
        {
            string currentVal;
            var currentRow = 1;
            var listSectionValues = new List<uint>();

            while (true)
            {
                currentVal = xLWorksheet.Cell(currentRow, 1).Value.ToString();

                if (string.IsNullOrEmpty(currentVal))
                {
                    break;
                }

                listSectionValues.Add(Convert.ToUInt32(currentVal));

                currentRow++;
            }

            return listSectionValues;
        }


        public static string[] GetFieldsFromSheet(IXLWorksheet xLWorksheet)
        {
            string currentVal;
            var currentRow = 1;
            var fieldValues = new List<string>();

            while (true)
            {
                currentVal = xLWorksheet.Cell(currentRow, 1).Value.ToString();

                if (string.IsNullOrEmpty(currentVal))
                {
                    break;
                }

                fieldValues.Add(currentVal.ToString());

                currentRow++;
            }

            return fieldValues.ToArray();
        }
    }
}