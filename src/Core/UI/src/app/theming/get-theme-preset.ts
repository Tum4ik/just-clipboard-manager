import { definePreset } from '@primeuix/themes';
import Aura from '@primeuix/themes/aura';
import { Preset } from "@primeuix/themes/types";

export function getThemePreset(): Preset {
  return definePreset(Aura, {
    components: {
      /* card: {
        colorScheme: {
          light: {
            root: {
              background: '{surface.100}'
            }
          },
          dark: {
            root: {
              background: '{surface.800}'
            }
          }
        }
      } */
    }
  });
}
