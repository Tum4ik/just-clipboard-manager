npm run copy-files-from-to
npm run tsc

cd ./plugins/text-plugin
#npm ci
npx rollup -c | Out-Host
cd ../../

npm run build
npm run electron-forge-make
