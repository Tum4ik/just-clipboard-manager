import { MakerDeb } from '@electron-forge/maker-deb';
import { MakerSquirrel } from '@electron-forge/maker-squirrel';
import type { ForgeConfig } from '@electron-forge/shared-types';

const makers = [];
if (process.platform == 'win32') {
  makers.push(new MakerSquirrel({}, ['win32']));
}
else if (process.platform == 'linux') {
  makers.push(new MakerDeb({}, ['linux']));
}

const config: ForgeConfig = {
  makers: makers,
  packagerConfig: {
    ignore: [
      '.github',
      '.vscode',
      'electron',
      '.editorconfig',
      '.gitignore',
      'angular.json',
      'app-icon.xcf',
      'cspell.json',
      'LICENSE',
      'README.md',
      'tsconfig.json',
      'tsconfig.app.json',
      'tsconfig.electron.json',
      'tsconfig.spec.json',
    ]
  }
};

export default config;
