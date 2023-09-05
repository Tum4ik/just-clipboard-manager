param (
  [string] $FilesDirectoryPath,
  [string] $PfxFilePath,
  [string] $PfxPassword,
  [string[]] $FileNamesToInclude=@()
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
if ($FileNamesToInclude.count -eq 0) {
  throw "The files to sign are missing. Use -FileNamesToInclude parameter."
}

$filesToSign = Get-ChildItem -Recurse `
  -Include $FileNamesToInclude `
  $FilesDirectoryPath `
  | Select-Object -ExpandProperty FullName
$signtool = "C:\Program Files (x86)\Windows Kits\10\bin\10.0.20348.0\x64\signtool.exe"
& $signtool sign /f $PfxFilePath /p $PfxPassword /fd SHA256 $filesToSign

if ($LastExitCode -ne 0) {
  throw
}
