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

        this.userSubscrip = this.userService.getByEmail(this.model.username).subscribe(
            userData => {
                Cookie.set('userInfo', JSON.stringify(userData));  
                this.menuService.currentUser = userData.name;
                this.menuService.user = userData;
            },
            error => this.errorHandlerService.handleErrors(error)
        );

        this.menuSubscrip = this.menuService.get(this.model.username).subscribe(
            data => {
                this.messageService.closeLoading();

                localStorage.setItem('menu', JSON.stringify(data));
                Cookie.set("currentUser", this.model.username);
                Cookie.set("currentUserMail", this.model.username);

                this.menuService.menu = data.menus;
                this.menuService.userIsDirector = data.isDirector; 
                this.menuService.userIsDaf = data.isDaf;
                this.menuService.userIsCdg = data.isCdg;

                this.menuService.dafMail = data.dafMail;
                this.menuService.cdgMail = data.cdgMail;
                this.menuService.pmoMail = data.pmoMail;
                this.menuService.rrhhMail = data.rrhhMail;
                this.menuService.sellerMail = data.sellerMail;

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
