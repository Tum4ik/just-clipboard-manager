param (
  [string] $Architecture,
  [string] $OutputDir
)

$outputFile = Join-Path $OutputDir $Architecture "efbundle.exe"
$project = Join-Path "Tum4ik.JustClipboardManager" "Tum4ik.JustClipboardManager.csproj"
$framework = "net7.0-windows"
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

$comProject = Join-Path `
  "Tum4ik.JustClipboardManager.COMImplementations" `
  "Tum4ik.JustClipboardManager.COMImplementations.csproj"
msbuild $comProject /t:Restore /t:Build `
  /p:Configuration=$configuration `
  /p:PublishProfile=FolderProfile_$Architecture `
  /p:Version=$Version

dotnet ef migrations bundle -f --self-contained `
  /p:IsMigrationsBundleCreation=true `
  -o $outputFile `
  -r $targetRuntime `
  -p $project `
  -s $project `
  --framework $framework `
  --configuration $configuration

if ($LastExitCode -ne 0) {
  throw
}
