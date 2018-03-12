import { Subscription } from 'rxjs/Subscription';
import { MessageService } from 'app/services/common/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { Message } from 'app/models/message';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { AuthenticationService } from "app/services/common/authentication.service";
import { MenuService } from "app/services/admin/menu.service";
import { UserService } from "app/services/admin/user.service";
import { CryptographyService } from 'app/services/common/cryptography.service';

@Component({
  selector: 'login',
  templateUrl: 'login.template.html'
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
        private authenticationService: AuthenticationService,
        private menuService: MenuService,
        private messageService: MessageService,
        private userService: UserService,
        private cryptoService: CryptographyService,
        private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() { 
        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }

    login() {
        this.messageService.showLoginLoading();

        this.loginSubscrip = this.authenticationService.login(this.model.username, this.model.password).subscribe(
            data => {
                this.onLoginSucces(data);
            },
            error => this.errorHandlerService.handleErrors(error));
    }

    onLoginSucces(data){
        Cookie.set('access_token', data.access_token);
        Cookie.set('refresh_token', data.refresh_token);

        this.userSubscrip = this.userService.getByEmail().subscribe(
            response => {
                const userData = response.data;
                Cookie.set('userInfo', JSON.stringify(userData));
                this.menuService.currentUser = userData.name;
                this.menuService.user = userData;
            },
            error => this.errorHandlerService.handleErrors(error)
        );

        this.menuSubscrip = this.menuService.get().subscribe(
            response => {
                const menu = response.data;
                this.messageService.closeLoading();

                localStorage.setItem('menu', JSON.stringify(menu));
                Cookie.set("currentUser", this.model.username);
                Cookie.set("currentUserMail", this.model.username);

                this.menuService.menu = menu.menus;
                this.menuService.userIsDirector = menu.isDirector;
                this.menuService.userIsDaf = menu.isDaf;
                this.menuService.userIsCdg = menu.isCdg;
                this.menuService.userIsRrhh = menu.isRrhh;

                this.menuService.dafMail = menu.dafMail;
                this.menuService.cdgMail = menu.cdgMail;
                this.menuService.pmoMail = menu.pmoMail;
                this.menuService.rrhhMail = menu.rrhhMail;
                this.menuService.sellerMail = menu.sellerMail;

                this.router.navigate([this.returnUrl]);
            },
            error => {
               this.errorHandlerService.handleErrors(error);
            }
        );
    }

    onSubmit(){
      this.login();
    }
}
