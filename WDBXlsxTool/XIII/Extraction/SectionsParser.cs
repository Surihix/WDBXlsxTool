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

            // Write whether the file is known
            currentSheet = wdbWorkbook.AddWorksheet("!!info");
            XlsxHelpers.WriteToCell(currentSheet, 1, 1, XlsxHelpers.WriteType.String, "IsKnown", true);

            if (WDBDicts.RecordIDs.ContainsKey(wdbVars.WDBName) && !wdbVars.IgnoreKnown)
            {
                wdbVars.IsKnown = true;
                XlsxHelpers.WriteToCell(currentSheet, 1, 2, XlsxHelpers.WriteType.Boolean, wdbVars.IsKnown, false);

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
                XlsxHelpers.WriteToCell(currentSheet, 1, 2, XlsxHelpers.WriteType.Boolean, wdbVars.IsKnown, false);
            }

            XlsxHelpers.AutoAdjustRowsAndColumns(currentSheet);

            // Parse and write strtypelist data
            currentSheet = wdbWorkbook.AddWorksheet(wdbVars.StrtypelistSectionName);
            wdbVars.StrtypelistValues = XlsxHelpers.WriteListSectionValues(wdbVars.StrtypelistData, currentSheet);

            // Parse and write typelist data
            currentSheet = wdbWorkbook.AddWorksheet(wdbVars.TypelistSectionName);
            wdbVars.TypelistValues = XlsxHelpers.WriteListSectionValues(wdbVars.TypelistData, currentSheet);

            // Write version data
            currentSheet = wdbWorkbook.AddWorksheet(wdbVars.VersionSectionName);
            XlsxHelpers.WriteToCell(currentSheet, 1, 1, XlsxHelpers.WriteType.UInt32, SharedMethods.DeriveUIntFromSectionData(wdbVars.VersionData, 0, true), false);
            XlsxHelpers.AutoAdjustRowsAndColumns(currentSheet);

            // Write structitem data
            if (wdbVars.IsKnown && !wdbVars.IgnoreKnown)
            {
                currentSheet = wdbWorkbook.AddWorksheet(wdbVars.StructItemSectionName);
                XlsxHelpers.WriteStructItemDataToSheet(currentSheet, wdbVars.FieldCount, wdbVars.Fields);
                XlsxHelpers.AutoAdjustRowsAndColumns(currentSheet);
            }
        }
    }
}