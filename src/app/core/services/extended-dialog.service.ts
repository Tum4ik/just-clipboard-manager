import { Directive, Injectable, Type } from '@angular/core';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { firstValueFrom } from 'rxjs';

@Injectable()
export class ExtendedDialogService extends DialogService {

  async openAsync<
    TComponent extends BaseDialog<TResult, TData>,
    TResult = TComponent extends BaseDialog<infer R, any> ? R : void,
    TData = TComponent extends BaseDialog<any, infer D> ? D : void
  >(
    component: Type<TComponent>, config: DynamicDialogConfig<TData>
  ): Promise<TResult> {
    const mergedConfig: DynamicDialogConfig<TData> = {
      modal: true,
      focusOnShow: false,
      dismissableMask: true,
      ...config
    };
    const dialogRef = this.open(component, mergedConfig);
    const dialogComponent = await firstValueFrom(dialogRef.onChildComponentLoaded.asObservable());
    const dynamicDialogComponent = this.getInstance(dialogRef);
    dialogComponent.onDialogLoaded(dynamicDialogComponent.data);
    return await firstValueFrom(dialogRef.onClose) as TResult;
  }
}


@Directive()
export abstract class BaseDialog<TResult, TData = void> {
  constructor(protected readonly dialogRef: DynamicDialogRef) { }

  abstract onDialogLoaded(data: TData): void;

  closeDialog(result: TResult) {
    this.dialogRef.close(result);
  }
}
