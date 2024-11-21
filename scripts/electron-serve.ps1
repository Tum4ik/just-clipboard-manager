dotnet publish `
  ./dotnet/JustClipboardManager.ClipboardListener/JustClipboardManager.ClipboardListener.csproj `
  --output ./dist/dotnet

copy-files-from-to
tsc -p tsconfig.electron.json

cd ./plugins/text-plugin
npx rollup -c
cd ../../

concurrently "ng serve" "wait-on tcp:4200 && electron . --serve"
