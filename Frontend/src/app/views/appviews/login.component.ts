import { Subscription } from 'rxjs/Subscription';
import { MessageService } from './../../services/message.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from "app/services/authentication.service";
import { MenuService } from "app/services/menu.service";

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

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private authenticationService: AuthenticationService,
        private menuService: MenuService,
        private messageService: MessageService) { }

    ngOnInit() {
        // reset login status
        this.authenticationService.logout();

        // get return url from route parameters or default to '/'
        this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || '/';
    }

    login() {
        this.loading = true;
        this.loginSubscrip = this.authenticationService.login(this.model.username, this.model.password).subscribe(
            data => {
                localStorage.setItem('currentUser', data.data.userName);
                localStorage.setItem('currentUserMail', data.data.email);
                this.menuService.currentUser = data.data.userName;

                this.menuSubscrip = this.menuService.get(this.menuService.currentUser).subscribe(
                    data => {
                        localStorage.setItem('menu', JSON.stringify(data));
                        this.menuService.menu = data;
                        this.router.navigate([this.returnUrl]);
                    },
                    error => {
                        this.loading = false;
                        if(error.messages) this.messageService.showMessages(error.messages);
                    }
                );
            },
            error => {
                this.loading = false;
                var json = JSON.parse(error._body)
                if(json.messages) this.messageService.showMessages(json.messages);
        });
    }

    onSubmit(){
      this.login();
    }
}
