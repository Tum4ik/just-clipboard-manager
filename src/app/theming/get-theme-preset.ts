import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';
import { Preset } from '@primeuix/themes/types';

export function getThemePreset(color: PresetColor): Preset {
  return definePreset(Aura, {
    semantic: {
      primary: {
        50: `{${color}.50}`,
        100: `{${color}.100}`,
        200: `{${color}.200}`,
        300: `{${color}.300}`,
        400: `{${color}.400}`,
        500: `{${color}.500}`,
        600: `{${color}.600}`,
        700: `{${color}.700}`,
        800: `{${color}.800}`,
        900: `{${color}.900}`,
        950: `{${color}.950}`
      }
    },
    components: {
      togglebutton: {
        colorScheme: {
          light: {
            root: {
              background: '{surface.200}',
              hoverBackground: '{surface.200}',
              checkedBackground: '{surface.200}',
            }
          }
        }
      }
    }
  });
}

export enum PresetColor {
  emerald = 'emerald',
  green = 'green',
  lime = 'lime',
  red = 'red',
  orange = 'orange',
  amber = 'amber',
  yellow = 'yellow',
  teal = 'teal',
  cyan = 'cyan',
  sky = 'sky',
  blue = 'blue',
  indigo = 'indigo',
  violet = 'violet',
  purple = 'purple',
  fuchsia = 'fuchsia',
  pink = 'pink',
  rose = 'rose',
  slate = 'slate',
  gray = 'gray',
  zinc = 'zinc',
  neutral = 'neutral',
  stone = 'stone',
}
