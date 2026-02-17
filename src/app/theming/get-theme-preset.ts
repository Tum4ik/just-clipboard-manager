import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';
import { Preset } from '@primeuix/themes/types';

export function getThemePreset(color: PresetColorName): Preset {
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

export type PresetColorName =
  | 'emerald'
  | 'green'
  | 'lime'
  | 'red'
  | 'orange'
  | 'amber'
  | 'yellow'
  | 'teal'
  | 'cyan'
  | 'sky'
  | 'blue'
  | 'indigo'
  | 'violet'
  | 'purple'
  | 'fuchsia'
  | 'pink'
  | 'rose'
  | 'slate'
  | 'gray'
  | 'zinc'
  | 'neutral'
  | 'stone'
  ;

export const PRESET_COLORS_MAP: Readonly<Record<PresetColorName, string>> = {
  'emerald': '#10b981',
  'green': '#22c55e',
  'lime': '#84cc16',
  'red': '#ef4444',
  'orange': '#f97316',
  'amber': '#f59e0b',
  'yellow': '#eab308',
  'teal': '#14b8a6',
  'cyan': '#06b6d4',
  'sky': '#0ea5e9',
  'blue': '#3b82f6',
  'indigo': '#6366f1',
  'violet': '#8b5cf6',
  'purple': '#a855f7',
  'fuchsia': '#d946ef',
  'pink': '#ec4899',
  'rose': '#f43f5e',
  'slate': '#64748b',
  'gray': '#6b7280',
  'zinc': '#71717a',
  'neutral': '#737373',
  'stone': '#78716c',
};
