import { AfterViewInit, Directive, input, inputBinding, Type, ViewContainerRef } from "@angular/core";
import { NavigationViewComponent } from "./navigation-view.component";

@Directive()
export abstract class NavigationView implements AfterViewInit {
  static readonly isNavigationView = true;

  constructor(
    private readonly viewContainer: ViewContainerRef,
  ) { }

  readonly nestedLevelTabId = input<string>();

  protected abstract readonly items: NavigationMenuItem[];

  ngAfterViewInit(): void {
    this.viewContainer.createComponent(NavigationViewComponent, {
      bindings: [
        inputBinding('items', () => this.items),
        inputBinding('nestedLevelTabId', this.nestedLevelTabId)
      ]
    });
  }
}

export interface NavigationMenuItem {
  id: string;
  label: string;
  icon: string;
  component: Type<any> | null;
}
