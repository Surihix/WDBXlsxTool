using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using WDBXlsxTool.Support;

namespace WDBXlsxTool.XIII.Extraction
{
    internal class RecordsParser
    {
        public static void ParseRecordsWithFields(BinaryReader wdbReader, WDBVariablesXIII wdbVars, XSSFWorkbook wdbWorkbook)
        {
            IRow currentRow;
            int currentRowIndex;
            int cellIndex;
            var currentSheet = wdbWorkbook.CreateSheet(wdbVars.SheetName);

            // Fill up all the fields
            currentRow = currentSheet.CreateRow(0);
            XlsxMethods.WriteToCell(currentRow, 0, "Records", XlsxMethods.CellObjects.String, true);
            cellIndex = 1;

            foreach (var field in wdbVars.Fields)
            {
                XlsxMethods.WriteToCell(currentRow, cellIndex, field, XlsxMethods.CellObjects.String, true);
                cellIndex++;
            }

            // Process each record's data
            var sectionPos = wdbReader.BaseStream.Position;
            string currentRecordName;
            byte[] currentRecordData;
            var strtypelistIndex = 0;
            var currentRecordDataIndex = 0;
            currentRowIndex = 1;

            for (int r = 0; r < wdbVars.RecordCount; r++)
            {
                cellIndex = 0;
                currentRow = currentSheet.CreateRow(currentRowIndex);

                _ = wdbReader.BaseStream.Position = sectionPos;
                currentRecordName = wdbReader.ReadBytesString(16, false);

                Console.WriteLine($"Record: {currentRecordName}");
                XlsxMethods.WriteToCell(currentRow, cellIndex, currentRecordName, XlsxMethods.CellObjects.String, false);
                cellIndex++;

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
                                            XlsxMethods.WriteToCell(currentRow, cellIndex, iTypedataVal, XlsxMethods.CellObjects.Int, false);
                                            cellIndex++;

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
                                            XlsxMethods.WriteToCell(currentRow, cellIndex, iTypedataVal, XlsxMethods.CellObjects.Int, false);
                                            cellIndex++;

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
                                            XlsxMethods.WriteToCell(currentRow, cellIndex, uTypeDataVal, XlsxMethods.CellObjects.UInt32, false);
                                            cellIndex++;

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
                                            XlsxMethods.WriteToCell(currentRow, cellIndex, uTypeDataVal, XlsxMethods.CellObjects.UInt32, false);
                                            cellIndex++;

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
                                            XlsxMethods.WriteToCell(currentRow, cellIndex, fTypeDataVal, XlsxMethods.CellObjects.Int, false);
                                            cellIndex++;

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
                                            XlsxMethods.WriteToCell(currentRow, cellIndex, fTypeDataVal, XlsxMethods.CellObjects.Int, false);
                                            cellIndex++;

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
                            XlsxMethods.WriteToCell(currentRow, cellIndex, floatDataVal, XlsxMethods.CellObjects.Float, false);
                            cellIndex++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            break;

                        // !!string section offset
                        case 2:
                            var stringDataOffset = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);
                            var derivedString = SharedMethods.DeriveStringFromArray(wdbVars.StringsData, (int)stringDataOffset);

                            Console.WriteLine($"{wdbVars.Fields[f]}: {derivedString}");
                            XlsxMethods.WriteToCell(currentRow, cellIndex, derivedString, XlsxMethods.CellObjects.String, false);
                            cellIndex++;

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
                                XlsxMethods.WriteToCell(currentRow, cellIndex, ulTypeDataVal, XlsxMethods.CellObjects.UInt64, false);
                                cellIndex++;

                                strtypelistIndex++;
                                currentRecordDataIndex += 8;
                                break;
                            }

                            var uintDataVal = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);

                            Console.WriteLine($"{wdbVars.Fields[f]}: {uintDataVal}");
                            XlsxMethods.WriteToCell(currentRow, cellIndex, uintDataVal, XlsxMethods.CellObjects.UInt32, false);
                            cellIndex++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            break;
                    }
                }

                currentRowIndex++;

                Console.WriteLine("");

                strtypelistIndex = 0;
                currentRecordDataIndex = 0;
                sectionPos += 32;
            }

            XlsxMethods.AutoSizeSheet(currentSheet, (int)wdbVars.RecordCount + 1);
        }


        public static void ParseRecordsWithoutFields(BinaryReader wdbReader, WDBVariablesXIII wdbVars, XSSFWorkbook wdbWorkbook)
        {
            IRow currentRow;
            int currentRowIndex;
            int cellIndex;
            var currentSheet = wdbWorkbook.CreateSheet(wdbVars.WDBName);

            // Fill up all the fields
            currentRow = currentSheet.CreateRow(0);
            XlsxMethods.WriteToCell(currentRow, 0, "Records", XlsxMethods.CellObjects.String, true);
            cellIndex = 1;

            var bitpackedFieldCounter = 0;
            var floatFieldCounter = 0;
            var stringFieldCounter = 0;
            var uintFieldCounter = 0;

            foreach (var strtypelistValue in wdbVars.StrtypelistValues)
            {
                switch (strtypelistValue)
                {
                    case 0:
                        XlsxMethods.WriteToCell(currentRow, cellIndex, $"bitpacked-field_{bitpackedFieldCounter}", XlsxMethods.CellObjects.String, true);
                        bitpackedFieldCounter++;
                        break;
                    case 1:
                        XlsxMethods.WriteToCell(currentRow, cellIndex, $"float-field_{floatFieldCounter}", XlsxMethods.CellObjects.String, true);
                        floatFieldCounter++;
                        break;
                    case 2:
                        XlsxMethods.WriteToCell(currentRow, cellIndex, $"!!string-field_{stringFieldCounter}", XlsxMethods.CellObjects.String, true);
                        stringFieldCounter++;
                        break;
                    case 3:
                        XlsxMethods.WriteToCell(currentRow, cellIndex, $"uint-field_{uintFieldCounter}", XlsxMethods.CellObjects.String, true);
                        uintFieldCounter++;
                        break;
                }

                cellIndex++;
            }

            // Process each record's data
            var sectionPos = wdbReader.BaseStream.Position;
            string currentRecordName;
            byte[] currentRecordData;
            var strtypelistIndex = 0;
            var currentRecordDataIndex = 0;
            currentRowIndex = 1;

            for (int r = 0; r < wdbVars.RecordCount; r++)
            {
                cellIndex = 0;
                currentRow = currentSheet.CreateRow(currentRowIndex);

                _ = wdbReader.BaseStream.Position = sectionPos;
                currentRecordName = wdbReader.ReadBytesString(16, false);

                Console.WriteLine($"Record: {currentRecordName}");
                XlsxMethods.WriteToCell(currentRow, cellIndex, currentRecordName, XlsxMethods.CellObjects.String, false);
                cellIndex++;

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
                            XlsxMethods.WriteToCell(currentRow, cellIndex, hexDataVal, XlsxMethods.CellObjects.String, false);
                            cellIndex++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            bitpackedFieldCounter++;
                            break;

                        // float value
                        case 1:
                            var floatDataVal = SharedMethods.DeriveFloatFromSectionData(currentRecordData, currentRecordDataIndex, true);

                            Console.WriteLine($"float-field_{floatFieldCounter}: {floatDataVal}");
                            XlsxMethods.WriteToCell(currentRow, cellIndex, floatDataVal, XlsxMethods.CellObjects.Float, false);
                            cellIndex++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            floatFieldCounter++;
                            break;

                        // !!string section offset
                        case 2:
                            var stringDataOffset = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);
                            var derivedString = SharedMethods.DeriveStringFromArray(wdbVars.StringsData, (int)stringDataOffset);

                            Console.WriteLine($"!!string-field_{stringFieldCounter}: {derivedString}");
                            XlsxMethods.WriteToCell(currentRow, cellIndex, derivedString, XlsxMethods.CellObjects.String, false);
                            cellIndex++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            stringFieldCounter++;
                            break;

                        // uint value
                        case 3:
                            var uintDataVal = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);

                            Console.WriteLine($"uint-field_{uintFieldCounter}: {uintDataVal}");
                            XlsxMethods.WriteToCell(currentRow, cellIndex, uintDataVal, XlsxMethods.CellObjects.UInt32, false);

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            uintFieldCounter++;
                            break;
                    }
                }

                currentRowIndex++;

                Console.WriteLine("");

                strtypelistIndex = 0;
                currentRecordDataIndex = 0;
                sectionPos += 32;
            }

            XlsxMethods.AutoSizeSheet(currentSheet, (int)wdbVars.RecordCount + 1);
        }
    }
}