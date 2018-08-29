import { Component } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {

  public spanishLang = 'es';
  public frenchLang = 'fr';
  public englishLang = 'en';
  public currLang = 'es';

  constructor(public translate: TranslateService) {
    translate.addLangs([this.spanishLang, this.englishLang, this.frenchLang]);
    translate.setDefaultLang(this.spanishLang);
    translate.use(this.spanishLang);
  }
}
