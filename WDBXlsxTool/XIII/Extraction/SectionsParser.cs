using ClosedXML.Excel;
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


        public static void MainSectionsToXlsx(WDBVariablesXIII wdbVars, XLWorkbook wdbWorkbook)
        {
            IXLWorksheet currentSheet;

            // Write basic info
            currentSheet = wdbWorkbook.AddWorksheet("!!info");

            XlsxWriterHelpers.WriteToCell(currentSheet, 1, 1, XlsxWriterHelpers.CellObjects.String, "records", true);
            XlsxWriterHelpers.WriteToCell(currentSheet, 1, 2, XlsxWriterHelpers.CellObjects.UInt32, wdbVars.RecordCount, false);

            XlsxWriterHelpers.WriteToCell(currentSheet, 2, 1, XlsxWriterHelpers.CellObjects.String, "IsKnown", true);

            if (WDBDicts.RecordIDs.ContainsKey(wdbVars.WDBName) && !wdbVars.IgnoreKnown)
            {
                wdbVars.IsKnown = true;
                XlsxWriterHelpers.WriteToCell(currentSheet, 2, 2, XlsxWriterHelpers.CellObjects.Boolean, wdbVars.IsKnown, false);

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
                XlsxWriterHelpers.WriteToCell(currentSheet, 2, 2, XlsxWriterHelpers.CellObjects.Boolean, wdbVars.IsKnown, false);
            }

            XlsxWriterHelpers.AutoAdjustRowsAndColumns(currentSheet);

            // Parse and write strtypelist data
            currentSheet = wdbWorkbook.AddWorksheet(wdbVars.StrtypelistSectionName);
            wdbVars.StrtypelistValues = WriteListSectionValues(wdbVars.StrtypelistData, currentSheet);

            // Parse and write typelist data
            currentSheet = wdbWorkbook.AddWorksheet(wdbVars.TypelistSectionName);
            wdbVars.TypelistValues = WriteListSectionValues(wdbVars.TypelistData, currentSheet);


            // Write version data
            currentSheet = wdbWorkbook.AddWorksheet(wdbVars.VersionSectionName);
            XlsxWriterHelpers.WriteToCell(currentSheet, 1, 1, XlsxWriterHelpers.CellObjects.UInt32, SharedMethods.DeriveUIntFromSectionData(wdbVars.VersionData, 0, true), false);
            XlsxWriterHelpers.AutoAdjustRowsAndColumns(currentSheet);

            // Write structitem data
            if (wdbVars.IsKnown && !wdbVars.IgnoreKnown)
            {
                currentSheet = wdbWorkbook.AddWorksheet(wdbVars.StructItemSectionName);
                var currentRow = 1;

                for (int i = 0; i < wdbVars.FieldCount; i++)
                {
                    XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxWriterHelpers.CellObjects.String, wdbVars.Fields[i], false);
                    currentRow++;
                }

                XlsxWriterHelpers.AutoAdjustRowsAndColumns(currentSheet);
            }
        }


        private static List<uint> WriteListSectionValues(byte[] listSectionData, IXLWorksheet currentSheet)
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

                XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxWriterHelpers.CellObjects.UInt32, listValue, false);
                currentRow++;

                listIndex += 4;
            }

            return listSectionValues;
        }
    }
}