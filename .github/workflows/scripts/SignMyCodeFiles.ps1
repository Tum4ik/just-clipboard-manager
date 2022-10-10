param (
  [string] $FilesDirectoryPath,
  [string] $PfxFilePath,
  [string] $PfxPassword
)

if (!$FilesDirectoryPath) {
  throw "The directory to sign files is not specified. Use -FilesDirectoryPath parameter."
}
if (!$PfxFilePath) {
  throw "The PFX file path is missing. Use -PfxFilePath parameter."
}
if (!$PfxPassword) {
  throw "The PFX password is missing. Use -PfxPassword parameter."
}

$filesToSign = Get-ChildItem -Recurse -Include *.dll,*.exe $FilesDirectoryPath `
  | Where { ! $_.PSIsContainer } `
  | Select-Object -ExpandProperty FullName
# 17763
$signtool = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.20348.0\x64\signtool.exe"
& $signtool sign /f $PfxFilePath /p $PfxPassword $filesToSign
