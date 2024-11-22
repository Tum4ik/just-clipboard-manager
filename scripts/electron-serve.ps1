dotnet publish `
  ./dotnet/JustClipboardManager.ClipboardListener/JustClipboardManager.ClipboardListener.csproj `
  --output ./dist/dotnet

npm run copy-files-from-to
npm run tsc

cd ./plugins/text-plugin
npx rollup -c
cd ../../

npm run concurrently "ng serve" "wait-on tcp:4200 && electron . --serve"
