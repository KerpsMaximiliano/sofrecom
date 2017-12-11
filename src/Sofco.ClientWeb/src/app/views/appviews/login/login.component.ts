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
        var userName = this.model.username.split("@")[0];
        this.messageService.showLoginLoading();

        this.loginSubscrip = this.authenticationService.login(userName, this.model.password).subscribe(
            data => {
                this.onLoginSucces(data);
            },
            error => this.errorHandlerService.handleErrors(error));
    }

    onLoginSucces(data){
        Cookie.set('access_token', data.access_token);
        Cookie.set('refresh_token', data.refresh_token);  

        var userName = this.model.username.split("@")[0];

        this.userSubscrip = this.userService.getByEmail(this.model.username).subscribe(
            userData => {
                Cookie.set('userInfo', JSON.stringify(userData));  
                this.menuService.currentUser = userData.name;
                this.menuService.user = userData;
            },
            error => this.errorHandlerService.handleErrors(error)
        );

        this.menuSubscrip = this.menuService.get(userName).subscribe(
            data => {
                this.messageService.closeLoading();

                localStorage.setItem('menu', JSON.stringify(data));
                Cookie.set("currentUser", userName);
                Cookie.set("currentUserMail", this.model.username);

                this.menuService.menu = data.menus;
                this.menuService.userIsDirector = data.isDirector;
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
