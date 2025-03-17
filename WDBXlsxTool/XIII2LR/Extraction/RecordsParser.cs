using ClosedXML.Excel;
using WDBXlsxTool.Support;

namespace WDBXlsxTool.XIII2LR.Extraction
{
    internal class RecordsParser
    {
        public static void ProcessRecords(BinaryReader wdbReader, WDBVariablesXIII2LR wdbVars, XLWorkbook wdbWorkbook)
        {
            var currentSheet = wdbWorkbook.AddWorksheet(wdbVars.SheetName);

            // Fill up all the fields
            XlsxHelpers.WriteToCell(currentSheet, 1, 1, XlsxHelpers.WriteType.String, "Records", true);

            var currentColumn = 2;  // horizontal

            foreach (var field in wdbVars.Fields)
            {
                XlsxHelpers.WriteToCell(currentSheet, 1, currentColumn, XlsxHelpers.WriteType.String, field, true);
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
                XlsxHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxHelpers.WriteType.String, currentRecordName, false);
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
                            uint strArrayTypeDataVal;
                            string strArrayTypeDictKey;
                            List<string> strArrayTypeDictList;
                            string strArrayTypeStringVal;

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
                                            XlsxHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxHelpers.WriteType.Int, iTypedataVal, false);
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
                                            XlsxHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxHelpers.WriteType.Int, iTypedataVal, false);
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
                                            XlsxHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxHelpers.WriteType.UInt32, uTypeDataVal, false);
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
                                            XlsxHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxHelpers.WriteType.UInt32, uTypeDataVal, false);
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
                                            XlsxHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxHelpers.WriteType.Int, fTypeDataVal, false);
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
                                            XlsxHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxHelpers.WriteType.Int, fTypeDataVal, false);
                                            currentColumn++;

                                            if (fieldBitsToProcess != 0)
                                            {
                                                f++;
                                            }
                                        }
                                        break;

                                    // (s#) strArray item index
                                    case "s":
                                        if (fieldNum > fieldBitsToProcess)
                                        {
                                            f--;
                                            fieldBitsToProcess = 0;
                                            continue;
                                        }
                                        else
                                        {
                                            binaryDataIndex -= fieldNum;

                                            strArrayTypeDataVal = BitOperationHelpers.BinaryToUInt(binaryData, binaryDataIndex, fieldNum);
                                            fieldBitsToProcess -= fieldNum;

                                            strArrayTypeDictKey = wdbVars.Fields[f];
                                            strArrayTypeDictList = wdbVars.StrArrayDict[strArrayTypeDictKey];

                                            if (strArrayTypeDataVal < strArrayTypeDictList.Count)
                                            {
                                                strArrayTypeStringVal = strArrayTypeDictList[(int)strArrayTypeDataVal];
                                            }
                                            else
                                            {
                                                strArrayTypeStringVal = "";
                                            }

                                            Console.WriteLine($"{strArrayTypeDictKey}: {strArrayTypeStringVal}");
                                            XlsxHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxHelpers.WriteType.String, strArrayTypeStringVal, false);
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
                            XlsxHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxHelpers.WriteType.Float, floatDataVal, false);
                            currentColumn++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            break;

                        // !!string section offset
                        case 2:
                            var stringDataOffset = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);
                            var derivedString = SharedMethods.DeriveStringFromArray(wdbVars.StringsData, (int)stringDataOffset);

                            Console.WriteLine($"{wdbVars.Fields[f]}: {derivedString}");
                            XlsxHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxHelpers.WriteType.String, derivedString, false);
                            currentColumn++;

                            strtypelistIndex++;
                            currentRecordDataIndex += 4;
                            break;

                        // uint value
                        case 3:
                            var uintDataVal = SharedMethods.DeriveUIntFromSectionData(currentRecordData, currentRecordDataIndex, true);

                            Console.WriteLine($"{wdbVars.Fields[f]}: {uintDataVal}");
                            XlsxHelpers.WriteToCell(currentSheet, currentRow, currentColumn, XlsxHelpers.WriteType.UInt32, uintDataVal, false);
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

            XlsxHelpers.AutoAdjustRowsAndColumns(currentSheet);
        }
    }
}