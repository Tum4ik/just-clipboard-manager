import { MakerDeb } from '@electron-forge/maker-deb';
import { MakerSquirrel } from '@electron-forge/maker-squirrel';
import { MakerZIP } from '@electron-forge/maker-zip';
import type { ForgeConfig } from '@electron-forge/shared-types';

const makers = [];
if (process.platform == 'win32') {
  makers.push(new MakerSquirrel({}, ['win32']));
}
else if (process.platform == 'linux') {
  makers.push(new MakerDeb({}, ['linux']));
}
else if (process.platform == 'darwin') {
  makers.push(new MakerZIP({}, ['darwin']));
}

const config: ForgeConfig = {
  makers: makers
};

export default config;
