import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { smoothlyMenu } from '../../../app.helpers';
import * as _ from 'lodash';
import { AuthenticationService } from "app/services/common/authentication.service";
import { Configuration } from "app/services/common/configuration";
import { MenuService } from "app/services/admin/menu.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { UserInfoService } from '../../../services/common/user-info.service';
declare var jQuery: any;

@Component({
  selector: 'topnavbar',
  templateUrl: 'topnavbar.template.html'
})
export class TopNavbarComponent {

  public userName: string;
  public employeeId: number;

  constructor(
        public configService: Configuration, 
        private translateService: TranslateService,
        private authService: AuthenticationService,
        private router: Router,
        public menuService: MenuService){

        var lang = 'es';

        if(lang){
          configService.setCurrLang(lang);
        }
        else{
          lang = translateService.getBrowserLang();
          configService.setCurrLang(lang);
        }

      this.userName = this.menuService.currentUser;

      this.setEmployeeId();
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
    window.location.reload();
  }

  logout(){
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  setEmployeeId() {
    const userInfo = UserInfoService.getUserInfo();

    if (userInfo && userInfo.employeeId && userInfo.name) {
        this.employeeId = userInfo.employeeId;
    }
  }

  goToProfile(){
    if(!this.employeeId){
      this.setEmployeeId();
    }

    this.router.navigate([`/profile/${this.employeeId}`]);
  }
}
