import { MenuService } from 'app/services/menu.service';
import { Router } from '@angular/router';
import { AuthenticationService } from './../../../services/authentication.service';
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

  constructor(
        public configService: Configuration, 
        private translateService: TranslateService,
        private authService: AuthenticationService,
        private router: Router,
        private menuService: MenuService
        ){
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

  logout(){
    this.authService.logout();
    this.router.navigate(['/login']);
  }

}
