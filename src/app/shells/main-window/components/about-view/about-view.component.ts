import { Component, inject, OnInit } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { MatTooltip } from '@angular/material/tooltip';
import { GoogleIcon } from "@app/core/components/google-icon/google-icon";
import { ThemeService } from '@app/core/services/theme.service';
import { TranslatePipe } from '@ngx-translate/core';
import { invoke } from '@tauri-apps/api/core';
import { Button, ButtonDirective } from 'primeng/button';
import { Divider } from 'primeng/divider';
import { Ripple } from "primeng/ripple";
import { ScrollViewComponent } from "../scroll-view/scroll-view.component";
import { BuyMeACoffee } from './buy-me-a-coffee/buy-me-a-coffee';
import { PrimeNgLogo } from './prime-ng-logo/prime-ng-logo';

@Component({
  selector: 'jcm-about-view',
  templateUrl: './about-view.component.html',
  styleUrl: './about-view.component.scss',
  imports: [
    ScrollViewComponent,
    Divider,
    ButtonDirective,
    Ripple,
    BuyMeACoffee,
    PrimeNgLogo,
    GoogleIcon,
    Button,
    MatTooltip,
    TranslatePipe,
  ],
})
export class AboutViewComponent implements OnInit {
  private readonly themeService = inject(ThemeService);

  readonly isDarkTheme = toSignal(this.themeService.isDarkTheme$);

  productName?: string | null;
  version?: string | null;
  email?: string | null;
  isEmailCopied = false;

  async ngOnInit() {
    this.productName = await invoke<string | null>('info_product_name');
    this.version = await invoke<string | null>('info_version');

    this.email = (await invoke<string | null>('info_authors'))
      ?.split(':')
      ?.map(author => author.match(/<([^>]+)>/))
      ?.find(match => match?.[1])
      ?.[1] ?? null;
  }


  async copyEmailToClipboard() {
    if (this.email) {
      await invoke('copy_text_to_clipboard', { text: this.email });
      this.isEmailCopied = true;
      setTimeout(() => this.isEmailCopied = false, 2000);
    }
  }
}
