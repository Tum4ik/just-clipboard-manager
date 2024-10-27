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
      '.vscode'
    ]
  }
};

export default config;
