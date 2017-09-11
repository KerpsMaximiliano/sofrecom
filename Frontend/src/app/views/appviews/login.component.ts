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
        private errorHandlerService: ErrorHandlerService) { }

    ngOnInit() {
        // reset login status
        this.authenticationService.logout();

        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }

    login() {
        this.loading = true;
        var usernameAzure = this.model.username.replace('sofrecom', 'tebrasofre.onmicrosoft');
        usernameAzure = usernameAzure.replace('.ar', '');

        this.loginSubscrip = this.authenticationService.login(usernameAzure, this.model.password).subscribe(
            data => {
                this.onLoginSucces(data);
            },
            error => {
                this.loading = false;
                var err = new Message("Usuario o Contraseña invalidos", 1);
                this.messageService.showMessages([err]);
        });
    }

    onLoginSucces(data){
        Cookie.set('access_token', `Bearer ${data.access_token}`);  
        Cookie.set('refresh_token', data.refresh_token);  

        this.userService.reloadHeaders();
        this.menuService.reloadHeaders();
        
        var userName = this.model.username.split("@")[0];

        this.userSubscrip = this.userService.getByEmail(this.model.username).subscribe(
            userData => {
                Cookie.set('userInfo', JSON.stringify(userData));  
                this.menuService.currentUser = userData.name;

            },
            error => this.errorHandlerService.handleErrors(error)
        );

        this.menuSubscrip = this.menuService.get(userName).subscribe(
            data => {
                localStorage.setItem('menu', JSON.stringify(data));
                Cookie.set("currentUser", userName);
                Cookie.set("currentUserMail", this.model.username);

                this.menuService.menu = data;
                this.router.navigate([this.returnUrl]);
            },
            error => {
               this.loading = false;
               this.errorHandlerService.handleErrors(error);
            }
        );
    }

    onSubmit(){
      this.login();
    }
}
