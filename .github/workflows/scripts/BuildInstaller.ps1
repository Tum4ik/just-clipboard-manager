param (
  [string] $Architecture,
  [string] $SigntoolPath,
  [string] $PfxFilePath,
  [string] $PfxPassword
)

$innoSetupSigntoolCmd = "$SigntoolPath sign /tr http://timestamp.digicert.com /fd SHA256 /td SHA256 /f $PfxFilePath /p $PfxPassword `$f"
"#define Architecture `"$Architecture`"" | Set-Content -Path .\InnoSetup\architecture.iss
.\inst\iscc.exe /S"MsSigntool=$innoSetupSigntoolCmd" ".\InnoSetup\Setup.iss" | Out-Host

if ($LastExitCode -ne 0) {
  throw
}
