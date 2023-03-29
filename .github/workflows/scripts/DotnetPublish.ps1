param (
  [string] $Architecture,
  [string] $Version
)

msbuild -restore /p:Configuration=Release /p:PublishProfile=FolderProfile_$Architecture
dotnet publish --no-build -c Release `
  /p:PublishProfile=FolderProfile_$Architecture `
  /p:Version=$Version

if ($LastExitCode -ne 0) {
  throw
}
