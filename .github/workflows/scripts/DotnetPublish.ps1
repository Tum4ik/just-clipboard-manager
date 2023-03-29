param (
  [string] $Architecture,
  [string] $Version
)

msbuild -restore `
  /t:Build /t:Publish `
  /p:Configuration=Release `
  /p:PublishProfile=FolderProfile_$Architecture `
  /p:Version=$Version
#dotnet publish --no-build -c Release `
#  /p:PublishProfile=FolderProfile_$Architecture `
#  /p:Version=$Version

if ($LastExitCode -ne 0) {
  throw
}
