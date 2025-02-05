import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { smoothlyMenu } from '../../../app.helpers';
import * as _ from 'lodash';
import { AuthenticationService } from "../../../services/common/authentication.service";
import { Configuration } from "../../../services/common/configuration";
import { MenuService } from "../../../services/admin/menu.service";
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
        private authService: AuthenticationService,
        private router: Router,
        public menuService: MenuService){

        // configService.setCurrLang('es');

        var lang =  Cookie.get("lang");

        if(lang){
          configService.setCurrLang(lang);
        }
        else{
          configService.setCurrLang('es');
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
