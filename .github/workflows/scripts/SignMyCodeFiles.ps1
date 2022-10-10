param (
  [string] $FilesDirectoryPath,
  [string] $PfxFilePath,
  [string] $PfxPassword
)

$filesToSign = Get-ChildItem -Recurse -Include *.dll,*.exe $FilesDirectoryPath | Where { ! $_.PSIsContainer } | Select-Object -ExpandProperty FullName
$signtool = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.17763.0\x86\signtool.exe"
& $signtool sign /f $PfxFilePath /p $PfxPassword $filesToSign
