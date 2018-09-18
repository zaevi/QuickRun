call C:\Dev\VS2017\Community\Common7\Tools\VsDevCmd.bat;
msbuild QuickRun.sln /p:Configuration=Release
mkdir Release
copy /y QuickRun\bin\Release\QuickRun.exe Release\QuickRun.exe
copy /y QuickRun.Setting\bin\Release\QuickRun.Setting.exe Release\QuickRun.Setting.exe