using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using WDBXlsxTool.Support;

namespace WDBXlsxTool.XIII.Extraction
{
    internal class SectionsParser
    {
        public static void MainSections(BinaryReader wdbReader, WDBVariablesXIII wdbVars)
        {
            // Parse main sections
            long currentSectionNamePos = 16;
            string sectioNameRead;

            while (true)
            {
                wdbReader.BaseStream.Position = currentSectionNamePos;
                sectioNameRead = wdbReader.ReadBytesString(16, false);

                // Break the loop if its
                // not a valid "!" section
                if (!sectioNameRead.StartsWith("!!"))
                {
                    _ = wdbReader.BaseStream.Position = currentSectionNamePos;
                    break;
                }

                // !!sheetname check
                if (sectioNameRead == "!!sheetname")
                {
                    SharedMethods.ErrorExit("Specified WDB file is from XIII-2 or LR. set the gamecode to -ff132 to extract this file.");
                }

                // !!strArray check
                if (sectioNameRead == "!!strArray")
                {
                    SharedMethods.ErrorExit("Specified WDB file is from XIII-2 or LR. set the gamecode to -ff132 to extract this file.");
                }

                // !!string
                if (sectioNameRead == wdbVars.StringSectionName)
                {
                    wdbVars.HasStringSection = true;

                    wdbVars.StringsData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                // !!strtypelist
                if (sectioNameRead == wdbVars.StrtypelistSectionName)
                {
                    wdbVars.StrtypelistData = SharedMethods.SaveSectionData(wdbReader, false);

                    if (wdbVars.StrtypelistData.Length != 0)
                    {
                        wdbVars.StrtypelistValues = SharedMethods.GetSectionDataValues(wdbVars.StrtypelistData);
                        wdbVars.FieldCount = (uint)wdbVars.StrtypelistValues.Count;
                    }

                    wdbVars.RecordCount--;
                }

                // !!typelist
                if (sectioNameRead == wdbVars.TypelistSectionName)
                {
                    wdbVars.TypelistData = SharedMethods.SaveSectionData(wdbReader, false);

                    if (wdbVars.TypelistData.Length != 0)
                    {
                        wdbVars.TypelistValues = SharedMethods.GetSectionDataValues(wdbVars.TypelistData);
                    }

                    wdbVars.RecordCount--;
                }

                // !!version
                if (sectioNameRead == wdbVars.VersionSectionName)
                {
                    wdbVars.VersionData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                currentSectionNamePos += 32;
            }

            // Check if the !!strtypelist
            // is parsed 
            if (wdbVars.StrtypelistData.Length == 0)
            {
                SharedMethods.ErrorExit("!!strtypelist section was not present in the file.");
            }
        }


        public static void MainSectionsToXlsx(WDBVariablesXIII wdbVars, XSSFWorkbook wdbWorkbook)
        {
            ISheet currentSheet;
            IRow currentRow;

            // Write basic info
            currentSheet = wdbWorkbook.CreateSheet("!!info");

            currentRow = currentSheet.CreateRow(0);
            XlsxMethods.WriteToCell(currentRow, 0, "records", XlsxMethods.CellObjects.String, false);
            XlsxMethods.WriteToCell(currentRow, 1, wdbVars.RecordCount, XlsxMethods.CellObjects.UInt, false);

            currentRow = currentSheet.CreateRow(1);
            XlsxMethods.WriteToCell(currentRow, 0, "IsKnown", XlsxMethods.CellObjects.String, false);

            if (WDBDicts.RecordIDs.ContainsKey(wdbVars.WDBName) && !wdbVars.IgnoreKnown)
            {
                wdbVars.IsKnown = true;
                XlsxMethods.WriteToCell(currentRow, 1, wdbVars.IsKnown, XlsxMethods.CellObjects.Boolean, false);

                wdbVars.SheetName = WDBDicts.RecordIDs[wdbVars.WDBName];

                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine($"sheetName: {wdbVars.SheetName}");
                Console.WriteLine("");
                Console.WriteLine("");

                wdbVars.FieldCount = (uint)WDBDicts.FieldNames[wdbVars.SheetName].Count;
                wdbVars.Fields = new string[wdbVars.FieldCount];

                // Write all of the field names 
                // if the file is fully known
                for (int sf = 0; sf < wdbVars.FieldCount; sf++)
                {
                    var derivedString = WDBDicts.FieldNames[wdbVars.SheetName][sf];
                    wdbVars.Fields[sf] = derivedString;
                }
            }
            else
            {
                XlsxMethods.WriteToCell(currentRow, 1, wdbVars.IsKnown, XlsxMethods.CellObjects.Boolean, false);
            }

            XlsxMethods.AutoSizeSheet(currentSheet, 1);

            // Parse and write strtypelist data
            currentSheet = wdbWorkbook.CreateSheet(wdbVars.StrtypelistSectionName);
            wdbVars.StrtypelistValues = WriteListSectionValues(wdbVars.StrtypelistData, currentSheet);

            // Parse and write typelist data
            currentSheet = wdbWorkbook.CreateSheet(wdbVars.TypelistSectionName);
            wdbVars.TypelistValues = WriteListSectionValues(wdbVars.TypelistData, currentSheet);

            // Write version data
            currentSheet = wdbWorkbook.CreateSheet(wdbVars.VersionSectionName);
            XlsxMethods.WriteToCell(currentSheet.CreateRow(0), 0, SharedMethods.DeriveUIntFromSectionData(wdbVars.VersionData, 0, true), XlsxMethods.CellObjects.UInt, false);
            XlsxMethods.AutoSizeSheet(currentSheet, 1);

            // Write structitem data
            if (wdbVars.IsKnown && !wdbVars.IgnoreKnown)
            {
                currentSheet = wdbWorkbook.CreateSheet(wdbVars.StructItemSectionName);
                var currentRowIndex = 0;

                for (int i = 0; i < wdbVars.FieldCount; i++)
                {
                    XlsxMethods.WriteToCell(currentSheet.CreateRow(currentRowIndex), 0, wdbVars.Fields[i], XlsxMethods.CellObjects.String, false);
                    currentRowIndex++;
                }

                XlsxMethods.AutoSizeSheet(currentSheet, (int)wdbVars.FieldCount);
            }
        }


        private static List<uint> WriteListSectionValues(byte[] listSectionData, ISheet currentSheet)
        {
            var listSectionValues = new List<uint>();

            var listIndex = 0;
            var currentlistData = new byte[4];
            var listValueCount = listSectionData.Length / 4;
            var currentRowIndex = 0;
            uint listValue;

            IRow currentRow;

            for (int s = 0; s < listValueCount; s++)
            {
                Array.ConstrainedCopy(listSectionData, listIndex, currentlistData, 0, 4);
                Array.Reverse(currentlistData);
                listValue = BitConverter.ToUInt32(currentlistData, 0);

                listSectionValues.Add(listValue);

                currentRow = currentSheet.CreateRow(currentRowIndex);
                XlsxMethods.WriteToCell(currentRow, 0, listValue, XlsxMethods.CellObjects.UInt, false);
                currentRowIndex++;

                listIndex += 4;
            }

            return listSectionValues;
        }
    }
}