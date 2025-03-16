using ClosedXML.Excel;

namespace WDBXlsxTool.XIII.Extraction
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
    }
}