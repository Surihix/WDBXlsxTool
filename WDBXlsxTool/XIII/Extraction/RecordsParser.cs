using ClosedXML.Excel;
using WDBXlsxTool.Support;

namespace WDBXlsxTool.XIII.Extraction
{
    internal class RecordsParser
    {
        public static void ParseRecordsWithFields(BinaryReader wdbReader, WDBVariablesXIII wdbVars, XLWorkbook wdbWorkbook)
        {
            var currentSheet = wdbWorkbook.AddWorksheet(wdbVars.SheetName);

            // Fill up all the fields
            XlsxWriterHelpers.WriteToCell(currentSheet, 1, 1, XlsxWriterHelpers.CellObjects.String, "Records", true);

            var currentColumn = 2;  // horizontal

            foreach (var field in wdbVars.Fields)
            {
                XlsxWriterHelpers.WriteToCell(currentSheet, 1, currentColumn, XlsxWriterHelpers.CellObjects.String, field, true);
                currentColumn++;
            }

            // Process each record's data
            var sectionPos = wdbReader.BaseStream.Position;
            string currentRecordName;
            var currentRow = 2;  // vertical
            byte[] currentRecordData;
            var strtypelistIndex = 0;
            var currentRecordDataIndex = 0;

            for (int r = 0; r < wdbVars.RecordCount; r++)
            {
                currentColumn = 1;

                _ = wdbReader.BaseStream.Position = sectionPos;
                currentRecordName = wdbReader.ReadBytesString(16, false);

                Console.WriteLine($"Record: {currentRecordName}");
                XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.String, currentRecordName, false);
                currentColumn++;

                currentRecordData = SharedMethods.SaveSectionData(wdbReader, false);

                for (int f = 0; f < wdbVars.FieldCount; f++)
                {
                    switch (wdbVars.StrtypelistValues[strtypelistIndex])
                    {
                        // bitpacked
                        case 0:
                            var binaryData = BitOperationHelpers.UIntToBinary(SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true));
                            var binaryDataIndex = binaryData.Length;
                            var fieldBitsToProcess = 32;

                            int iTypedataVal;
                            uint uTypeDataVal;
                            int fTypeDataVal;

                            while (fieldBitsToProcess != 0 && f < wdbVars.FieldCount)
                            {
                                var fieldType = wdbVars.Fields[f].Substring(0, 1);
                                var fieldNum = SharedMethods.DeriveFieldNumber(wdbVars.Fields[f]);

                                switch (fieldType)
                                {
                                    // sint
                                    case "i":
                                        if (fieldNum == 0)
                                        {
                                            iTypedataVal = BitOperationHelpers.BinaryToInt(binaryData, binaryDataIndex - 32, 32);
                                            fieldBitsToProcess = 0;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {iTypedataVal}");
                                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.Int, iTypedataVal, false);
                                            currentColumn++;

                                            break;
                                        }
                                        if (fieldNum > fieldBitsToProcess)
                                        {
                                            f--;
                                            fieldBitsToProcess = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            binaryDataIndex -= fieldNum;

                                            iTypedataVal = BitOperationHelpers.BinaryToInt(binaryData, binaryDataIndex, fieldNum);
                                            fieldBitsToProcess -= fieldNum;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {iTypedataVal}");
                                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.Int, iTypedataVal, false);
                                            currentColumn++;

                                            if (fieldBitsToProcess != 0)
                                            {
                                                f++;
                                            }
                                        }
                                        break;

                                    // uint 
                                    case "u":
                                        if (fieldNum == 0)
                                        {
                                            uTypeDataVal = BitOperationHelpers.BinaryToUInt(binaryData, binaryDataIndex - 32, 32);
                                            fieldBitsToProcess = 0;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {uTypeDataVal}");
                                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.UInt32, uTypeDataVal, false);
                                            currentColumn++;

                                            break;
                                        }
                                        if (fieldNum > fieldBitsToProcess)
                                        {
                                            f--;
                                            fieldBitsToProcess = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            binaryDataIndex -= fieldNum;

                                            uTypeDataVal = BitOperationHelpers.BinaryToUInt(binaryData, binaryDataIndex, fieldNum);
                                            fieldBitsToProcess -= fieldNum;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {uTypeDataVal}");
                                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.UInt32, uTypeDataVal, false);
                                            currentColumn++;

                                            if (fieldBitsToProcess != 0)
                                            {
                                                f++;
                                            }
                                        }
                                        break;

                                    // float (bitpacked as int)
                                    case "f":
                                        if (fieldNum == 0)
                                        {
                                            fTypeDataVal = BitOperationHelpers.BinaryToInt(binaryData, binaryDataIndex - 32, 32);
                                            fieldBitsToProcess = 0;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {fTypeDataVal}");
                                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.Int, fTypeDataVal, false);
                                            currentColumn++;

                                            break;
                                        }
                                        if (fieldNum > fieldBitsToProcess)
                                        {
                                            f--;
                                            fieldBitsToProcess = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            binaryDataIndex -= fieldNum;

                                            fTypeDataVal = BitOperationHelpers.BinaryToInt(binaryData, binaryDataIndex, fieldNum);
                                            fieldBitsToProcess -= fieldNum;

                                            Console.WriteLine($"{wdbVars.Fields[f]}: {fTypeDataVal}");
                                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.Int, fTypeDataVal, false);
                                            currentColumn++;

                                            if (fieldBitsToProcess != 0)
                                            {
                                                f++;
                                            }
                                        }
                                        break;
                                }
                            }

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            break;

                        // float value
                        case 1:
                            var floatDataVal = SharedMethods.DeriveFloatFromSectionData(currentRecordData, currentRecordDataIndex, true);

                            Console.WriteLine($"{wdbVars.Fields[f]}: {floatDataVal}");
                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.Float, floatDataVal, false);
                            currentColumn++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            break;

                        // !!string section offset
                        case 2:
                            var stringDataOffset = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);
                            var derivedString = SharedMethods.DeriveStringFromArray(wdbVars.StringsData, (int)stringDataOffset);

                            Console.WriteLine($"{wdbVars.Fields[f]}: {derivedString}");
                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.String, derivedString, false);
                            currentColumn++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            break;

                        // uint value
                        case 3:
                            if (wdbVars.Fields[f].StartsWith("u64"))
                            {
                                var processArray = new byte[8];
                                Array.ConstrainedCopy(currentRecordData, currentRecordDataIndex, processArray, 0, 8);
                                Array.Reverse(processArray);

                                var ulTypeDataVal = BitConverter.ToUInt64(processArray, 0);

                                Console.WriteLine($"{wdbVars.Fields[f]}(uint64): {ulTypeDataVal}");
                                XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.UInt64, ulTypeDataVal, false);
                                currentColumn++;

                                strtypelistIndex += 2;
                                currentRecordDataIndex += 8;
                                break;
                            }

                            var uintDataVal = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);

                            Console.WriteLine($"{wdbVars.Fields[f]}: {uintDataVal}");
                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.UInt32, uintDataVal, false);
                            currentColumn++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            break;
                    }
                }

                currentRow++;
                Console.WriteLine("");

                strtypelistIndex = 0;
                currentRecordDataIndex = 0;
                sectionPos += 32;
            }

            XlsxWriterHelpers.AutoAdjustRowsAndColumns(currentSheet);
        }


        public static void ParseRecordsWithoutFields(BinaryReader wdbReader, WDBVariablesXIII wdbVars, XLWorkbook wdbWorkbook)
        {
            var currentSheet = wdbWorkbook.AddWorksheet(wdbVars.WDBName);

            // Fill up all the fields
            XlsxWriterHelpers.WriteToCell(currentSheet, 1, 1, XlsxWriterHelpers.CellObjects.String, "Records", true);

            var bitpackedFieldCounter = 0;
            var floatFieldCounter = 0;
            var stringFieldCounter = 0;
            var uintFieldCounter = 0;

            var currentColumn = 2;  // horizontal

            foreach (var strtypelistValue in wdbVars.StrtypelistValues)
            {
                switch (strtypelistValue)
                {
                    case 0:
                        XlsxWriterHelpers.WriteToCell(currentSheet, 1, currentColumn, XlsxWriterHelpers.CellObjects.String, $"bitpacked-field_{bitpackedFieldCounter}", true);
                        bitpackedFieldCounter++;
                        break;

                    case 1:
                        XlsxWriterHelpers.WriteToCell(currentSheet, 1, currentColumn, XlsxWriterHelpers.CellObjects.String, $"float-field_{floatFieldCounter}", true);
                        floatFieldCounter++;
                        break;

                    case 2:
                        XlsxWriterHelpers.WriteToCell(currentSheet, 1, currentColumn, XlsxWriterHelpers.CellObjects.String, $"!!string-field_{stringFieldCounter}", true);
                        stringFieldCounter++;
                        break;

                    case 3:
                        XlsxWriterHelpers.WriteToCell(currentSheet, 1, currentColumn, XlsxWriterHelpers.CellObjects.String, $"uint-field_{uintFieldCounter}", true);
                        uintFieldCounter++;
                        break;
                }

                currentColumn++;
            }

            // Process each record's data
            var sectionPos = wdbReader.BaseStream.Position;
            string currentRecordName;
            var currentRow = 2;  // vertical
            byte[] currentRecordData;
            var strtypelistIndex = 0;
            var currentRecordDataIndex = 0;

            for (int r = 0; r < wdbVars.RecordCount; r++)
            {
                currentColumn = 1;

                _ = wdbReader.BaseStream.Position = sectionPos;
                currentRecordName = wdbReader.ReadBytesString(16, false);

                Console.WriteLine($"Record: {currentRecordName}");
                XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.String, currentRecordName, false);
                currentColumn++;

                currentRecordData = SharedMethods.SaveSectionData(wdbReader, false);

                bitpackedFieldCounter = 0;
                floatFieldCounter = 0;
                stringFieldCounter = 0;
                uintFieldCounter = 0;

                for (int f = 0; f < wdbVars.FieldCount; f++)
                {
                    switch (wdbVars.StrtypelistValues[strtypelistIndex])
                    {
                        // bitpacked
                        case 0:
                            var bitpackedData = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);
                            var hexDataVal = "0x" + bitpackedData.ToString("X").PadLeft(8, '0');

                            Console.WriteLine($"bitpacked-field_{bitpackedFieldCounter}: {hexDataVal}");
                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.String, hexDataVal, false);
                            currentColumn++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            bitpackedFieldCounter++;
                            break;

                        // float value
                        case 1:
                            var floatDataVal = SharedMethods.DeriveFloatFromSectionData(currentRecordData, currentRecordDataIndex, true);

                            Console.WriteLine($"float-field_{floatFieldCounter}: {floatDataVal}");
                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.Float, floatDataVal, false);
                            currentColumn++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            floatFieldCounter++;
                            break;

                        // !!string section offset
                        case 2:
                            var stringDataOffset = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);
                            var derivedString = SharedMethods.DeriveStringFromArray(wdbVars.StringsData, (int)stringDataOffset);

                            Console.WriteLine($"!!string-field_{stringFieldCounter}: {derivedString}");
                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.String, derivedString, false);
                            currentColumn++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            stringFieldCounter++;
                            break;

                        // uint value
                        case 3:
                            var uintDataVal = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);

                            Console.WriteLine($"uint-field_{uintFieldCounter}: {uintDataVal}");
                            XlsxWriterHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxWriterHelpers.CellObjects.UInt32, uintDataVal, false);
                            currentColumn++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            uintFieldCounter++;
                            break;
                    }
                }

                currentRow++;
                Console.WriteLine("");

                strtypelistIndex = 0;
                currentRecordDataIndex = 0;
                sectionPos += 32;
            }

            XlsxWriterHelpers.AutoAdjustRowsAndColumns(currentSheet);
        }
    }
}