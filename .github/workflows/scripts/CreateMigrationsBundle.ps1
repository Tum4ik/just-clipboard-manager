param (
  [string] $Architecture,
  [string] $OutputDir
)

$outputFile = Join-Path $OutputDir $Architecture "efbundle.exe"
$project = Join-Path "Tum4ik.JustClipboardManager" "Tum4ik.JustClipboardManager.csproj"
$framework = "net6.0-windows"
$configuration = "Release"
if ($Architecture -eq "x64") {
  $targetRuntime = "win-x64"
}
elseif ($Architecture -eq "x86") {
  $targetRuntime = "win-x86"
}
else {
  throw "Unsupported target runtime."
}

$result = dotnet ef migrations bundle -f --self-contained `
  -o $outputFile `
  -r $targetRuntime `
  --runtime $targetRuntime `
  -p $project `
  -s $project `
  --framework $framework `
  --configuration $configuration

if ($LastExitCode -ne 0) {
  throw $result
}
