import { Component } from '@angular/core';
import { BaseDialog } from '@app/core/services/extended-dialog.service';
import { TranslatePipe } from '@ngx-translate/core';
import { Button } from 'primeng/button';

@Component({
  selector: 'jcm-confirm-plugin-uninstall',
  templateUrl: './confirm-plugin-uninstall.html',
  styleUrl: './confirm-plugin-uninstall.scss',
  imports: [
    TranslatePipe,
    Button,
  ]
})
export class ConfirmPluginUninstall extends BaseDialog<ConfirmPluginUninstallResult> {
  onDialogLoaded(data: void): void {
  }

  protected cancel() {
    this.closeDialog(ConfirmPluginUninstallResult.Cancel);
  }

  protected removePluginAndClips() {
    this.closeDialog(ConfirmPluginUninstallResult.RemovePluginAndClips);
  }

  protected removeOnlyPlugin() {
    this.closeDialog(ConfirmPluginUninstallResult.RemoveOnlyPlugin);
  }
}


export enum ConfirmPluginUninstallResult {
  Cancel, RemoveOnlyPlugin, RemovePluginAndClips
}
