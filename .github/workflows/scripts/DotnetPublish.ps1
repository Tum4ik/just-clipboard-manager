param (
  [string] $Architecture,
  [string] $Version
)

msbuild /t:Restore /t:Build /t:Publish `
  /p:Configuration=Release `
  /p:PublishProfile=FolderProfile_$Architecture `
  /p:Version=$Version

if ($LastExitCode -ne 0) {
  throw
}
