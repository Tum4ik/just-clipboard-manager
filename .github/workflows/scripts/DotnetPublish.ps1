param (
  [string] $Architecture,
  [string] $Version
)

dotnet publish -c Release `
  /p:PublishProfile=FolderProfile_$Architecture `
  /p:Version=$Version

if ($LastExitCode -ne 0) {
  throw
}
