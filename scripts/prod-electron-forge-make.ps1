npm run copy-files-from-to
tsc -p tsconfig.electron.json

npm run build
electron-forge make
