import { Component } from '@angular/core';
import { ScrollViewComponent } from "../../../scroll-view/scroll-view.component";
import { AutoDeleteClips } from './auto-delete-clips/auto-delete-clips';
import { AutoStartApplication } from './auto-start-application/auto-start-application';

@Component({
  selector: 'jcm-general-settings',
  templateUrl: './general-settings.component.html',
  styleUrl: './general-settings.component.scss',
  imports: [
    ScrollViewComponent,
    AutoStartApplication,
    AutoDeleteClips,
  ]
})
export class GeneralSettingsComponent {

}
