import { MakerDeb } from '@electron-forge/maker-deb';
import { MakerSquirrel } from '@electron-forge/maker-squirrel';
import type { ForgeConfig } from '@electron-forge/shared-types';
import fs from 'fs';
import path from 'path';

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
  },
  hooks: {
    async packageAfterPrune(config, buildPath): Promise<void> {
      fs.rmSync(path.join(buildPath, 'electron'), { recursive: true, force: true });
      fs.rmSync(path.join(buildPath, 'src'), { recursive: true, force: true });
    }
  }
};

export default config;
