using ClosedXML.Excel;

namespace WDBXlsxTool.Support
{
    internal class XlsxWriterHelpers
    {
        public static void WriteToCell(IXLWorksheet xLWorksheet, int row, int column, CellObjects cellObjects, object valueToWrite, bool bold)
        {
            switch (cellObjects)
            {
                case CellObjects.Int:
                    xLWorksheet.Cell(row, column).Value = Convert.ToInt32(valueToWrite);
                    break;

                case CellObjects.Float:
                    xLWorksheet.Cell(row, column).Value = valueToWrite.ToString();
                    break;

                case CellObjects.String:
                    xLWorksheet.Cell(row, column).Value = valueToWrite.ToString();
                    break;

                case CellObjects.UInt32:
                    xLWorksheet.Cell(row, column).Value = Convert.ToUInt32(valueToWrite);
                    break;

                case CellObjects.UInt64:
                    xLWorksheet.Cell(row, column).Value = Convert.ToUInt64(valueToWrite);
                    break;

                case CellObjects.Boolean:
                    xLWorksheet.Cell(row, column).Value = valueToWrite.ToString();
                    break;
            }

            xLWorksheet.Cell(row, column).Style.Font.Bold = bold;
            xLWorksheet.Cell(row, column).Style.Font.FontName = "Arial";
            xLWorksheet.Cell(row, column).Style.Font.FontSize = 12;
        }

        public enum CellObjects
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

                WriteToCell(currentSheet, currentRow, 1, CellObjects.UInt32, listValue, false);
                currentRow++;

                listIndex += 4;
            }

            return listSectionValues;
        }
    }
}