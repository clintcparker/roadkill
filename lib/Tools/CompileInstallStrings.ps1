$env:Path = "C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\Bin\"
cd ..\..\src\Roadkill.Core\Localization\Text
& resgen /compile "InstallStrings.txt,InstallStrings.resx"
move -Path InstallStrings.resx ..\Resx\ -Force
cd ..\..\..\..\lib\tools