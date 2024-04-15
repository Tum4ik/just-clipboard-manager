param (
  [string] $Architecture,
  [string] $Version
)

msbuild /t:Restore /t:Build /t:Publish `
  /p:Configuration=Release `
  /p:Platform=$Architecture `
  /p:PublishDir=bin\publish\$Architecture\ `
  /p:PublishProtocol=FileSystem `
  /p:SelfContained=true `
  /p:RuntimeIdentifier=win-$Architecture `
  /p:PublishSingleFile=false `
  /p:PublishReadyToRun=false `
  /p:Version=$Version `
  /p:SentryOrg=tum4ik `
  /p:SentryProject=just-clipboard-manager `
  /p:SentryUploadSymbols=true `
  /p:SentryUploadSources=true

if ($LastExitCode -ne 0) {
  throw
}
