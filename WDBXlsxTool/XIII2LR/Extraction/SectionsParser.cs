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
                wdbVars.SheetName = "Not Specified";
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine($"{wdbVars.SheetNameSectionName}: {wdbVars.SheetName}");
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

            XlsxWriterHelpers.WriteToCell(currentSheet, 1, 1, XlsxWriterHelpers.CellObjects.String, "records", true);
            XlsxWriterHelpers.WriteToCell(currentSheet, 1, 2, XlsxWriterHelpers.CellObjects.UInt32, wdbVars.RecordCount, false);

            XlsxWriterHelpers.WriteToCell(currentSheet, 2, 1, XlsxWriterHelpers.CellObjects.String, "hasStrArray", true);
            XlsxWriterHelpers.WriteToCell(currentSheet, 2, 2, XlsxWriterHelpers.CellObjects.Boolean, wdbVars.HasStrArraySection, false);

            var currentRow = 3;

            // Write array info values
            // if strArray section is
            // present
            if (wdbVars.HasStrArraySection)
            {
                XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxWriterHelpers.CellObjects.String, "bitsPerOffset", true);
                XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, 2, XlsxWriterHelpers.CellObjects.UInt32, wdbVars.BitsPerOffset, false);
                currentRow++;

                XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxWriterHelpers.CellObjects.String, "offsetsPerValue", true);
                XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, 2, XlsxWriterHelpers.CellObjects.UInt32, wdbVars.OffsetsPerValue, false);
                currentRow++;
            }

            XlsxWriterHelpers.AutoAdjustRowsAndColumns(currentSheet);

            // Parse and write strtypelistData
            // and also write typelistData if
            // strtypelist is v1
            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxWriterHelpers.CellObjects.String, "isStrTypelistV1", true);
            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, 2, XlsxWriterHelpers.CellObjects.Boolean, wdbVars.ParseStrtypelistAsV1, false);

            if (wdbVars.ParseStrtypelistAsV1)
            {
                currentSheet = wdbWorkbook.AddWorksheet(wdbVars.StrtypelistSectionName);
                wdbVars.StrtypelistValues = XlsxWriterHelpers.WriteListSectionValues(wdbVars.StrtypelistData, currentSheet);

                currentSheet = wdbWorkbook.AddWorksheet(wdbVars.TypelistSectionName);
                wdbVars.TypelistValues = XlsxWriterHelpers.WriteListSectionValues(wdbVars.TypelistData, currentSheet);
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

                    XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxWriterHelpers.CellObjects.UInt32, strtypelistValue, false);
                    currentRow++;

                    strtypelistbIndex++;
                }
            }

            // Write version data
            currentSheet = wdbWorkbook.AddWorksheet(wdbVars.VersionSectionName);
            XlsxWriterHelpers.WriteToCell(currentSheet, 1, 1, XlsxWriterHelpers.CellObjects.UInt32, SharedMethods.DeriveUIntFromSectionData(wdbVars.VersionData, 0, true), false);
            XlsxWriterHelpers.AutoAdjustRowsAndColumns(currentSheet);

            // Write structitem data
            currentSheet = wdbWorkbook.AddWorksheet(wdbVars.StructItemSectionName);
            currentRow = 1;

            for (int i = 0; i < wdbVars.FieldCount; i++)
            {
                XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, 1, XlsxWriterHelpers.CellObjects.String, wdbVars.Fields[i], false);
                currentRow++;
            }

            XlsxWriterHelpers.AutoAdjustRowsAndColumns(currentSheet);
        }
    }
}