import { AfterViewInit, Directive, ViewContainerRef } from "@angular/core";
import { Router } from "@angular/router";
import { MenuItem } from "primeng/api";
import { NavigationViewComponent } from "./navigation-view.component";

@Directive()
export abstract class NavigationView implements AfterViewInit {
  constructor(
    private readonly router: Router,
    private readonly viewContainer: ViewContainerRef,
  ) { }

  private activatedHref?: string;

  abstract items: MenuItem[];

  ngAfterViewInit(): void {
    const component = this.viewContainer.createComponent(NavigationViewComponent);
    component.setInput('items', this.items);
    component.instance.hrefActivated.subscribe(this.onHrefActivated.bind(this));
  }

  /* onRouterAttached(): void {
    if (this.activatedHref) {
      this.router.navigateByUrl(this.activatedHref);
    }
  } */

  private onHrefActivated(href: string) {
    this.activatedHref = href;
  }
}
