import { ComponentRef, Directive } from "@angular/core";
import { ActivatedRoute, RouterOutlet } from "@angular/router";

@Directive({
  selector: 'jcm-notifying-router-outlet'
})
export class NotifyingRouterOutlet extends RouterOutlet {
  override attach(ref: ComponentRef<any>, activatedRoute: ActivatedRoute): void {
    (ref.instance as OnRouterAttached)?.onRouterAttached?.();
    super.attach(ref, activatedRoute);
  }

  override detach(): ComponentRef<any> {
    (this.component as OnRouterDetached)?.onRouterDetached?.();
    return super.detach();
  }
}


export interface OnRouterAttached {
  onRouterAttached(): void;
}

export interface OnRouterDetached {
  onRouterDetached(): void;
}
