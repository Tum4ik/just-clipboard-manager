import { inject } from "@angular/core";
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';

export function registerSvgIcons() {
  addSvgIconsSet('flags', '/assets/icons/flags-set.svg');
}

function addSvgIconsSet(namespace: string, path: string) {
  const matIconRegistry = inject(MatIconRegistry);
  const domSanitizer = inject(DomSanitizer);
  matIconRegistry.addSvgIconSetInNamespace(
    namespace,
    domSanitizer.bypassSecurityTrustResourceUrl(path)
  );
}
