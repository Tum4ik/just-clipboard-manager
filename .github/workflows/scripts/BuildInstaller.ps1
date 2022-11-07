param (
  [string] $Architecture
)

"#define Architecture `"$Architecture`"" | Set-Content -Path .\InnoSetup\architecture.iss
.\inst\iscc.exe ".\InnoSetup\Setup.iss" | Out-Null

if ($LastExitCode -ne 0) {
  throw
}
