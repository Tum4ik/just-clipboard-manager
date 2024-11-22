npm run copy-files-from-to
npm run tsc

cd ./plugins/text-plugin
npx rollup -c
cd ../../

npm run build
npm run electron-forge -- make
