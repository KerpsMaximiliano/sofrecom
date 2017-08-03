import { TranslateService } from '@ngx-translate/core';
import { Configuration } from 'app/services/configuration';
import { Component } from '@angular/core';
import { smoothlyMenu } from '../../../app.helpers';
import * as _ from 'lodash';
declare var jQuery:any;

@Component({
  selector: 'topnavbar',
  templateUrl: 'topnavbar.template.html'
})
export class TopNavbarComponent {

  constructor(public configService: Configuration, private translateService: TranslateService){
      let browserLang = translateService.getBrowserLang();
      configService.setCurrLang(browserLang);
  }

  toggleNavigation(): void {
    jQuery("body").toggleClass("mini-navbar");
    smoothlyMenu();
  }

  capitalize(str): string{
    return _.capitalize(str);
  }

  selectLanguage(lang){
    this.configService.setCurrLang(lang);
  }

}
