# WDBXlsxTool
This program allows you to convert the WDB database files from the FF13 game trilogy to (.xlsx) excel file as well as allow you to convert the xlsx file, back to WDB. the program should be launched from a command prompt terminal with a few argument switches to perform the conversion functions. a list of valid argument switches are given below:

**Game Codes:**
<br>``-ff131`` For FF13-1 WDB files
<br>``-ff132`` For FF13-2 and FF13-LR 's WDB files

<br>**Tool actions:**
<br>``-x`` Converts the WDB file's data into a new xlsx file
<br>``-xi`` Converts the WDB file's data into a new xlsx file without the fieldnames (only when gamecode is -ff131)
<br>``-c`` Converts the data in the xlsx file into a new WDB file
<br>``-?`` Display the help page. will also display few argument examples too.

<br>Commandline usage examples:
<br>``WDBXlsxTool.exe -? ``
<br>``WDBXlsxTool.exe -ff131 -x "auto_clip.wdb" ``
<br>``WDBXlsxTool.exe -ff131 -xi "auto_clip.wdb" ``
<br>``WDBXlsxTool.exe -ff131 -c "auto_clip.xlsx" ``

## Important notes
- The program requires .net 6.0 Desktop Runtime to be installed on your PC. you can get it from this [page](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).

- The WDB file or xlsx file has to be specified after the game code and the tool action argument switches.

- When using the `-ff131` game code switch, you can use the `-xi` tool action argument switch, to prevent generating field names for the records when its converted to xlsx file.

- Its highly recommended to use Microsoft excel to edit/view the xlsx files. you can try other softwares, but do know that if it doesn't follow the xlsx format that excel uses, then there is a chance for tool to fail or mess up somewhere during the WDB conversion process.

- Field names will be present in the xlsx file only for some WDB files from 13-1. refer to this [page](https://github.com/LR-Research-Team/Datalog/wiki/WDB-Field-Names) for information about the field names.

- Floating point values will be written as string in the xlsx file and this is done to ensure that there is no precision loss in the value when its written to the xlsx file. you can however edit and add in your values correctly in the proper floating point format and the tool will parse the value as float when converting back to WDB format.

- If you are adding new records in the WDB file, then make sure to add your new records according to the alphabetical order. this would require adding them in between two existing records or after the last record.

- When editing numerical fields, make sure that your number does not exceed the bit amount value given in the field name.

- Please report any problems that you encounter with the converted xlsx/WDB files by opening an issue page detailing the issue here or in the "Fabula Nova Crystallis: Modding Community" discord server.

## For developers
- The following package was used for writing to xlsx format:
<br>**ClosedXML** - https://github.com/ClosedXML/ClosedXML

- Refer to this [page](https://github.com/LR-Research-Team/Datalog/wiki/WDB) for information about the file structure of the WDB file.
