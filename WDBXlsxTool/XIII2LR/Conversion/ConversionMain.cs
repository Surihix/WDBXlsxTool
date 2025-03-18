namespace WDBXlsxTool.XIII2LR.Conversion
{
    internal class ConversionMain
    {
        public static void StartConversion(string inXlsxFile)
        {
            var wdbVars = new WDBVariablesXIII2LR();

            XlsxDeserializer.DeserializeData(inXlsxFile, wdbVars);

            Console.WriteLine("");

            if (wdbVars.HasSheetName)
            {
                Console.WriteLine($"{wdbVars.SheetNameSectionName}: {wdbVars.SheetName}");
            }

            Console.WriteLine($"Total records (with sections): {wdbVars.RecordCountWithSections}");
            Console.WriteLine("");

            wdbVars.WDBFilePath = Path.Combine(Path.GetDirectoryName(inXlsxFile), Path.GetFileNameWithoutExtension(inXlsxFile) + ".wdb");

            if (wdbVars.HasStrArraySection)
            {
                RecordsConversion.ConvertRecordsStrArray(wdbVars);
            }
            else
            {
                RecordsConversion.ConvertRecords(wdbVars);
            }

            WDBbuilder.BuildWDB(wdbVars);

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Finished building wdb file for extracted xlsx data");
        }
    }
}