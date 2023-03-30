param (
  [string] $Architecture,
  [string] $Version
)

$project = Join-Path `
  "Tum4ik.JustClipboardManager.COMImplementations" `
  "Tum4ik.JustClipboardManager.COMImplementations.csproj"

msbuild $project /t:Restore /t:Build `
  /p:Configuration=Release `
  /p:Platform=$Architecture `
  /p:PublishProfile=FolderProfile_$Architecture `
  /p:Version=$Version
dotnet publish -c Release `
  /p:PublishProfile=FolderProfile_$Architecture `
  /p:Version=$Version

if ($LastExitCode -ne 0) {
  throw
}
