dotnet publish `
  ./dotnet/JustClipboardManager.ClipboardListener/JustClipboardManager.ClipboardListener.csproj `
  --output ./dist/dotnet

npm run copy-files-from-to
tsc -p tsconfig.electron.json

cd ./plugins/text-plugin
npx rollup -c
cd ../../

npm run build
electron .
