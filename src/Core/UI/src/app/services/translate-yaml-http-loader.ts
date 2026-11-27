import { HttpClient } from '@angular/common/http';
import { inject, Service } from '@angular/core';
import { TranslateLoader, TranslationObject } from '@ngx-translate/core';
import { map, Observable } from 'rxjs';
import { parse } from 'yaml';

@Service()
export class TranslateYamlHttpLoader implements TranslateLoader {
  private readonly http = inject(HttpClient);

  getTranslation(lang: string): Observable<TranslationObject> {
    return this.http
      .get(`/assets/i18n/${lang}.yml`, { responseType: 'text' })
      .pipe(map(data => parse(data)));
  }
}
