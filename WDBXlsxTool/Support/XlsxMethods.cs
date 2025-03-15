using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace WDBXlsxTool.Support
{
    internal class XlsxMethods
    {
        private static XSSFCellStyle FieldCellStyle { get; set; }
        private static XSSFCellStyle ValueCellStyle { get; set; }

        public static void SetCellStyles(XSSFWorkbook wdbWorkbook)
        {
            var wdbWorkbookfont = (XSSFFont)wdbWorkbook.CreateFont();
            wdbWorkbookfont.FontHeightInPoints = 11;
            wdbWorkbookfont.FontName = "Arial";
            wdbWorkbookfont.IsBold = true;

            FieldCellStyle = (XSSFCellStyle)wdbWorkbook.CreateCellStyle();
            FieldCellStyle.SetFont(wdbWorkbookfont);

            wdbWorkbookfont = (XSSFFont)wdbWorkbook.CreateFont();
            wdbWorkbookfont.FontHeightInPoints = 11;
            wdbWorkbookfont.FontName = "Arial";
            wdbWorkbookfont.IsBold = false;

            ValueCellStyle = (XSSFCellStyle)wdbWorkbook.CreateCellStyle();
            ValueCellStyle.SetFont(wdbWorkbookfont);
        }


        public static void WriteToCell(IRow CurrentRow, int CellIndex, object value, CellObjects cellObjects, bool isField)
        {
            var cell = CurrentRow.CreateCell(CellIndex);

            switch (cellObjects)
            {
                case CellObjects.Int:
                    cell.SetCellValue(Convert.ToInt32(value));
                    break;

                case CellObjects.Float:
                    cell.SetCellValue((double)Convert.ToDouble(value));
                    break;

                case CellObjects.String:
                    cell.SetCellValue((string)value);
                    break;

                case CellObjects.UInt32:
                    cell.SetCellValue(Convert.ToUInt32(value));
                    break;

                case CellObjects.UInt64:
                    cell.SetCellValue(Convert.ToUInt64(value));
                    break;

                case CellObjects.Boolean:
                    cell.SetCellValue((bool)value);
                    break;
            }

            if (isField)
            {
                cell.CellStyle = FieldCellStyle;
            }
            else
            {
                cell.CellStyle = ValueCellStyle;
            }
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


        public static void AutoSizeSheet(ISheet sheetName, int valuesCount)
        {
            for (int i = 0; i <= valuesCount; i++)
            {
                sheetName.AutoSizeColumn(i);
            }
        }
    }
}