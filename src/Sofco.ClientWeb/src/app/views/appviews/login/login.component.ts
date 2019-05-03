import { Subscription } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { AuthenticationService } from "../../../services/common/authentication.service";
import { MenuService } from "../../../services/admin/menu.service";
import { UserService } from "../../../services/admin/user.service";
import { UserInfoService } from '../../../services/common/user-info.service';
import { MessageService } from '../../../services/common/message.service';

@Component({
  selector: 'app-login',
  templateUrl: 'login.template.html',
  styleUrls: ['login.component.scss']
})
export class LoginComponent implements OnInit {
    model: any = {};
    loading = false;
    returnUrl: string;

    loginSubscrip: Subscription;
    menuSubscrip: Subscription;
    userSubscrip: Subscription;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private messageService: MessageService,
        private authenticationService: AuthenticationService,
        private menuService: MenuService,
        private userService: UserService) { }

    ngOnInit() {
    }

    login() {
        this.loading = true;

        this.loginSubscrip = this.authenticationService.login(this.model.username, this.model.password)
        .subscribe(
            data => { this.onLoginSucces(data); },
            (response) => {
                var json = JSON.parse(response._body)
                if(json.messages) this.messageService.showMessages(json.messages);
                this.loading = false;
            });
    }

    onLoginSucces(data) {
        Cookie.set('access_token', data.accessToken);
        Cookie.set('refresh_token', data.refreshToken);

        this.userSubscrip = this.userService.getByEmail().subscribe(
            response => {
                const userData = response;
                const userName = userData.name;
                UserInfoService.setUserInfo(userData);
                this.menuService.currentUser = userName;
                this.menuService.user = userData;

                this.getMenuData();
            });
    }

    getMenuData(){
        this.menuSubscrip = this.menuService.get().subscribe(
            response => {
                const menu = response.data;
                this.loading = false;

                localStorage.setItem('menu', JSON.stringify(menu));
                const username = this.model.username.split('@')[0];
                Cookie.set("currentUser", username);
                Cookie.set("currentUserMail", username);
 
                this.menuService.menu = menu.menus;
                this.menuService.userIsDirector = menu.isDirector;
                this.menuService.userIsManager = menu.isManager;
                this.menuService.userIsDaf = menu.isDaf;
                this.menuService.userIsCdg = menu.isCdg;
                this.menuService.userIsRrhh = menu.isRrhh;
                this.menuService.userIsCompliance = menu.isCompliance;
                this.menuService.userIsGaf = menu.isGaf;

                this.menuService.dafMail = menu.dafMail;
                this.menuService.cdgMail = menu.cdgMail;
                this.menuService.pmoMail = menu.pmoMail;
                this.menuService.rrhhMail = menu.rrhhMail;
                this.menuService.sellerMail = menu.sellerMail;
                this.menuService.areaIds = menu.areaIds;
                this.menuService.sectorIds = menu.sectorIds;

                this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || `/profile/${this.menuService.user.employeeId}`;

                this.router.navigate([this.returnUrl]);
            });
    }

    onSubmit(){
      this.login();
    }
}
