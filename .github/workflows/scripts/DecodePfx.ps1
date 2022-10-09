param (
  [string] $Base64Pfx,
  [string] $OutputPath
)

if (!$Base64Pfx) {
  throw "The Base64 encoded pfx data is missing. Use -Base64Pfx parameter."
}
if (!$OutputPath) {
  throw "The output file path is missing. Use -OutputPath parameter."
}

$pfxCertBytes = [System.Convert]::FromBase64String($Base64Pfx)
[IO.File]::WriteAllBytes($OutputPath, $pfxCertBytes)
