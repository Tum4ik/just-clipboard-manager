param (
  [string] $RuntimeIdentifier,
  [string] $Version
)

dotnet publish ./dotnet/JustClipboardManager.ClipboardListener/JustClipboardManager.ClipboardListener.csproj `
  -c Release `
  -r $RuntimeIdentifier `
  --self-contained `
  --output ./dist/dotnet `
  --nologo `
  /p:Version=$Version
