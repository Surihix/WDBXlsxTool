using WDBXlsxTool.Support;

namespace WDBXlsxTool
{
    internal class Core
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("");

            try
            {
                if (args.Length == 0)
                {
                    SharedMethods.ErrorExit("Enough arguments not specified. please launch the program with -? switch for more info.");
                }

                if (args[0] == "-?")
                {
                    Console.WriteLine("Game Codes:");
                    Console.WriteLine("-ff131 = Sets conversion compatibility to FFXIII");
                    Console.WriteLine("-ff132 = Sets conversion compatibility to FFXIII-2 and LR");

                    Console.WriteLine("");
                    Console.WriteLine("Tool actions:");
                    Console.WriteLine("-? = Display this help page");
                    Console.WriteLine("-x = Converts the wdb data into a new xlsx file");
                    Console.WriteLine("-xi = Converts the wdb data into a new xlsx file without the fieldnames (only when gamecode is -ff131)");
                    Console.WriteLine("-c = Converts the wdb data in the xlsx file, to a new wdb file");

                    Console.WriteLine("");
                    Console.WriteLine("Examples (with -ff131 game code):");
                    Console.WriteLine("WDBXlsxTool.exe -?");
                    Console.WriteLine("WDBXlsxTool.exe -ff131 -x \"auto_clip.wdb\"");
                    Console.WriteLine("WDBXlsxTool.exe -ff131 -xi \"auto_clip.wdb\"");
                    Console.WriteLine("WDBXlsxTool.exe -ff131 -c \"auto_clip.xlsx\"");

                    Console.ReadLine();
                    Environment.Exit(0);
                }

                if (args.Length < 3)
                {
                    SharedMethods.ErrorExit("Enough arguments not specified for this process");
                }

                // Assign the gameCode
                if (Enum.TryParse(args[0].Replace("-", ""), false, out GameCodes gameCode) == false)
                {
                    SharedMethods.ErrorExit("Specified game code was invalid");
                }

                // Assign the tool action
                if (Enum.TryParse(args[1].Replace("-", ""), false, out ToolActions toolAction) == false)
                {
                    SharedMethods.ErrorExit("Specified tool action was invalid");
                }

                var inFile = args[2];

                if (!File.Exists(inFile))
                {
                    SharedMethods.ErrorExit("Specified WDB or xlsx file is missing");
                }

                switch (toolAction)
                {
                    case ToolActions.x:
                    case ToolActions.xi:
                        if (gameCode == GameCodes.ff131)
                        {
                            XIII.Extraction.ExtractionMain.StartExtraction(inFile, toolAction == ToolActions.xi);
                        }
                        else
                        {
                            XIII2LR.Extraction.ExtractionMain.StartExtraction(inFile);
                        }
                        break;


                    case ToolActions.c:
                        if (gameCode == GameCodes.ff131)
                        {
                            XIII.Conversion.ConversionMain.StartConversion(inFile);
                        }
                        else
                        {
                            XIII2LR.Conversion.ConversionMain.StartConversion(inFile);
                        }
                        break;


                    default:
                        SharedMethods.ErrorExit("Specified tool action is invalid");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("An exception has occured!");
                Console.WriteLine("");
                Console.WriteLine($"{ex}");

                Environment.Exit(2);
            }
        }


        private enum GameCodes
        {
            ff131,
            ff132
        }


        private enum ToolActions
        {
            x,
            xi,
            c
        }
    }
}