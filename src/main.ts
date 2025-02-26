import { bootstrapApplication } from "@angular/platform-browser";
import { invoke } from "@tauri-apps/api/core";
import { AppComponent } from "./app/app.component";
import { appConfig } from "./app/app.config";

bootstrapApplication(AppComponent, appConfig).catch((err) => {
  console.error(err);
  invoke('sentry_capture_error', { message: err.stack ?? err });
});
