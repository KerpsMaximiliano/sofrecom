import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { Component } from '@angular/core';
import { smoothlyMenu } from '../../../app.helpers';
import * as _ from 'lodash';
import { AuthenticationService } from "app/services/common/authentication.service";
import { Configuration } from "app/services/common/configuration";
import { MenuService } from "app/services/admin/menu.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
declare var jQuery: any;

@Component({
  selector: 'topnavbar',
  templateUrl: 'topnavbar.template.html'
})
export class TopNavbarComponent {

  public userName: string;

  constructor(
        public configService: Configuration, 
        private translateService: TranslateService,
        private authService: AuthenticationService,
        private router: Router,
        public menuService: MenuService){

        var lang = Cookie.get("lang");

        if(lang){
          configService.setCurrLang(lang);
        }
        else{
          lang = translateService.getBrowserLang();
          configService.setCurrLang(lang);
        }

      this.userName = this.menuService.currentUser;
  }

  toggleNavigation(): void {
    jQuery("body").toggleClass("mini-navbar");
    smoothlyMenu();
  }

  capitalize(str): string{
    return _.capitalize(str);
  }

  selectLanguage(lang){
    Cookie.set("lang", lang);
    this.configService.setCurrLang(lang);
  }

  logout(){
    this.authService.logout();
    this.router.navigate(['/login']);
  }

}
