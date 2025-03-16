using ClosedXML.Excel;
using WDBXlsxTool.Support;

namespace WDBXlsxTool.XIII2LR.Extraction
{
    internal class ExtractionMain
    {
        public static void StartExtraction(string inWDBfile)
        {
            var wdbVars = new WDBVariablesXIII2LR();

            using (var wdbReader = new BinaryReader(File.Open(inWDBfile, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                wdbVars.XlsxFilePath = Path.Combine(Path.GetDirectoryName(inWDBfile), Path.GetFileNameWithoutExtension(inWDBfile) + ".xlsx");

                _ = wdbReader.BaseStream.Position = 0;
                if (wdbReader.ReadBytesString(3, false) != "WPD")
                {
                    SharedMethods.ErrorExit("Not a valid WPD file");
                }

                _ = wdbReader.BaseStream.Position += 1;
                wdbVars.RecordCount = wdbReader.ReadBytesUInt32(true);

                if (wdbVars.RecordCount == 0)
                {
                    SharedMethods.ErrorExit("No records/sections are present in this file");
                }

                SectionsParser.MainSections(wdbReader, wdbVars);

                Console.WriteLine("");
                Console.WriteLine($"Total records: {wdbVars.RecordCount}");
                Console.WriteLine("");

                using (var wdbWorkbook = new XLWorkbook())
                {
                    SectionsParser.MainSectionsToXlsx(wdbVars, wdbWorkbook);

                    Console.WriteLine("Parsing records....");
                    Console.WriteLine("");
                    Thread.Sleep(1000);

                    RecordsParser.ProcessRecords(wdbReader, wdbVars, wdbWorkbook);

                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("Writing wdb data to xlsx file....");

                    if (File.Exists(wdbVars.XlsxFilePath))
                    {
                        File.Delete(wdbVars.XlsxFilePath);
                    }

                    wdbWorkbook.SaveAs(wdbVars.XlsxFilePath);
                }
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Finished extracting wdb data to xlsx file");
        }
    }
}