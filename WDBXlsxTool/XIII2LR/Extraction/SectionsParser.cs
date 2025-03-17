using ClosedXML.Excel;
using System.Text;
using WDBXlsxTool.Support;

namespace WDBXlsxTool.XIII2LR.Extraction
{
    internal class SectionsParser
    {
        public static void MainSections(BinaryReader wdbReader, WDBVariablesXIII2LR wdbVars)
        {
            // Parse main sections
            long currentSectionNamePos = 16;
            string sectioNameRead;

            wdbVars.StrtypelistData = new byte[] { };
            wdbVars.StructItemData = new byte[] { };
            wdbVars.FieldCount = 0;

            while (true)
            {
                wdbReader.BaseStream.Position = currentSectionNamePos;
                sectioNameRead = wdbReader.ReadBytesString(16, false);

                // Break the loop if its
                // not a valid "!" section
                if (!sectioNameRead.StartsWith("!"))
                {
                    _ = wdbReader.BaseStream.Position = currentSectionNamePos;
                    break;
                }

                // !!sheetname
                if (sectioNameRead == wdbVars.SheetNameSectionName)
                {
                    wdbVars.HasSheetName = true;
                    _ = wdbReader.BaseStream.Position = wdbReader.ReadBytesUInt32(true);
                    wdbVars.SheetName = wdbReader.ReadStringTillNull();
                    wdbVars.RecordCount--;
                }

                // !!strArray
                if (sectioNameRead == wdbVars.StrArraySectionName)
                {
                    wdbVars.HasStrArraySection = true;

                    _ = wdbReader.BaseStream.Position = currentSectionNamePos;
                    StrArrayParser.SubSections(wdbReader, wdbVars);
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
                    wdbVars.ParseStrtypelistAsV1 = true;
                    wdbVars.StrtypelistData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                // !!strtypelistb
                if (sectioNameRead == wdbVars.StrtypelistbSectionName)
                {
                    wdbVars.ParseStrtypelistAsV1 = false;
                    wdbVars.StrtypelistData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                // !!typelist
                if (sectioNameRead == wdbVars.TypelistSectionName)
                {
                    wdbVars.HasTypelistSection = true;
                    wdbVars.TypelistData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                // !!version
                if (sectioNameRead == wdbVars.VersionSectionName)
                {
                    wdbVars.VersionData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                // !structitem
                if (sectioNameRead == wdbVars.StructItemSectionName)
                {
                    wdbVars.StructItemData = SharedMethods.SaveSectionData(wdbReader, false);
                    wdbVars.RecordCount--;
                }

                // !structitemnum
                if (sectioNameRead == wdbVars.StructItemNumSectionName)
                {
                    wdbVars.FieldCount = BitConverter.ToUInt32(SharedMethods.SaveSectionData(wdbReader, true), 0);
                    wdbVars.RecordCount--;
                }

                currentSectionNamePos += 32;
            }

            // Check if the important 
            // sections are all parsed
            var imptSectionsParsed = wdbVars.StrtypelistData.Length != 0 && wdbVars.StructItemData.Length != 0 && wdbVars.FieldCount != 0;

            if (!imptSectionsParsed)
            {
                SharedMethods.ErrorExit("Necessary sections were unable to be processed correctly.");
            }

            if (wdbVars.SheetName == "" || wdbVars.SheetName == null)
            {
                wdbVars.SheetName = "!!datasheet";
                wdbVars.HasSheetName = false;
            }

            Console.WriteLine("");
            Console.WriteLine("");

            if (wdbVars.HasSheetName)
            {
                Console.WriteLine($"{wdbVars.SheetNameSectionName}: {wdbVars.SheetName}");
            }
            else
            {
                Console.WriteLine($"{wdbVars.SheetNameSectionName}: Not Specified");
            }

            Console.WriteLine("");
            Console.WriteLine("");

            // Process !structitem data
            wdbVars.Fields = new string[wdbVars.FieldCount];
            var stringStartPos = 0;

            for (int sf = 0; sf < wdbVars.FieldCount; sf++)
            {
                var derivedString = SharedMethods.DeriveStringFromArray(wdbVars.StructItemData, stringStartPos);

                if (derivedString == "")
                {
                    SharedMethods.ErrorExit("Detected a null string structitem.");
                }

                wdbVars.Fields[sf] = derivedString;

                stringStartPos += Encoding.UTF8.GetByteCount(derivedString) + 1;
            }

            // Process strArray sections
            // data
            if (wdbVars.HasStrArraySection)
            {
                Console.WriteLine($"Organizing {wdbVars.StrArraySectionName} data....");

                StrArrayParser.ArrangeArrayData(wdbVars);

                Console.WriteLine("");
                Console.WriteLine("");
            }
        }


        public static void MainSectionsToXlsx(WDBVariablesXIII2LR wdbVars, XLWorkbook wdbWorkbook)
        {
            IXLWorksheet currentSheet;

            // Write basic info
            currentSheet = wdbWorkbook.AddWorksheet("!!info");

            XlsxHelpers.WriteToCell(currentSheet, 1, 1, XlsxHelpers.WriteType.String, "records", true);
            XlsxHelpers.WriteToCell(currentSheet, 1, 2, XlsxHelpers.WriteType.UInt32, wdbVars.RecordCount, false);

            XlsxHelpers.WriteToCell(currentSheet, 2, 1, XlsxHelpers.WriteType.String, "hasSheetName", true);
            XlsxHelpers.WriteToCell(currentSheet, 2, 2, XlsxHelpers.WriteType.Boolean, wdbVars.HasSheetName, false);

            XlsxHelpers.WriteToCell(currentSheet, 3, 1, XlsxHelpers.WriteType.String, "hasStrArray", true);
            XlsxHelpers.WriteToCell(currentSheet, 3, 2, XlsxHelpers.WriteType.Boolean, wdbVars.HasStrArraySection, false);

            var currentRow = 4;

            // Write array info values
            // if strArray section is
            // present
            if (wdbVars.HasStrArraySection)
            {
                XlsxHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxHelpers.WriteType.String, "bitsPerOffset", true);
                XlsxHelpers.WriteToCell(currentSheet, currentRow, 2, XlsxHelpers.WriteType.UInt32, wdbVars.BitsPerOffset, false);
                currentRow++;

                XlsxHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxHelpers.WriteType.String, "offsetsPerValue", true);
                XlsxHelpers.WriteToCell(currentSheet, currentRow, 2, XlsxHelpers.WriteType.UInt32, wdbVars.OffsetsPerValue, false);
                currentRow++;
            }

            XlsxHelpers.AutoAdjustRowsAndColumns(currentSheet);

            // Determine strtypelist version
            XlsxHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxHelpers.WriteType.String, "isStrTypelistV1", true);
            XlsxHelpers.WriteToCell(currentSheet, currentRow, 2, XlsxHelpers.WriteType.Boolean, wdbVars.ParseStrtypelistAsV1, false);
            currentRow++;

            // Determine whether typelist exists
            XlsxHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxHelpers.WriteType.String, "hasTypelist", true);
            XlsxHelpers.WriteToCell(currentSheet, currentRow, 2, XlsxHelpers.WriteType.Boolean, wdbVars.HasTypelistSection, false);

            // Parse and write strtypelist data
            if (wdbVars.ParseStrtypelistAsV1)
            {
                currentSheet = wdbWorkbook.AddWorksheet(wdbVars.StrtypelistSectionName);
                wdbVars.StrtypelistValues = XlsxHelpers.WriteListSectionValues(wdbVars.StrtypelistData, currentSheet);
            }
            else
            {
                currentSheet = wdbWorkbook.AddWorksheet(wdbVars.StrtypelistbSectionName);

                currentRow = 1;
                var strtypelistbIndex = 0;
                uint strtypelistValue;

                for (int s = 0; s < wdbVars.StrtypelistData.Length; s++)
                {
                    strtypelistValue = wdbVars.StrtypelistData[strtypelistbIndex];
                    wdbVars.StrtypelistValues.Add(strtypelistValue);

                    XlsxHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxHelpers.WriteType.UInt32, strtypelistValue, false);
                    currentRow++;

                    strtypelistbIndex++;
                }
            }

            // Parse and write typelist data
            if (wdbVars.HasTypelistSection)
            {
                currentSheet = wdbWorkbook.AddWorksheet(wdbVars.TypelistSectionName);
                wdbVars.TypelistValues = XlsxHelpers.WriteListSectionValues(wdbVars.TypelistData, currentSheet);
            }

            // Write version data
            currentSheet = wdbWorkbook.AddWorksheet(wdbVars.VersionSectionName);
            XlsxHelpers.WriteToCell(currentSheet, 1, 1, XlsxHelpers.WriteType.UInt32, SharedMethods.DeriveUIntFromSectionData(wdbVars.VersionData, 0, true), false);
            XlsxHelpers.AutoAdjustRowsAndColumns(currentSheet);

            // Write structitem data
            currentSheet = wdbWorkbook.AddWorksheet(wdbVars.StructItemSectionName);
            currentRow = 1;

            for (int i = 0; i < wdbVars.FieldCount; i++)
            {
                XlsxHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxHelpers.WriteType.String, wdbVars.Fields[i], false);
                currentRow++;
            }

            XlsxHelpers.AutoAdjustRowsAndColumns(currentSheet);
        }
    }
}