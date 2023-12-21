param (
  [string] $SigntoolPath,
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
& $SigntoolPath sign `
  /tr http://timestamp.digicert.com `
  /fd SHA256 `
  /td SHA256 `
  /f $PfxFilePath `
  /p $PfxPassword `
  $filesToSign

if ($LastExitCode -ne 0) {
  throw
}
