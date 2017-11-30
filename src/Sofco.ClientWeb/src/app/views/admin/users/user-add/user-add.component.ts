import { Router } from '@angular/router';
import { Component, OnDestroy } from '@angular/core';
import { MessageService } from 'app/services/common/message.service';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { UserService } from 'app/services/admin/user.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-user-add',
  templateUrl: './user-add.component.html',
  styleUrls: ['./user-add.component.scss']
})
export class UserAddComponent implements OnDestroy {


    public model: any = { userPrincipalName: "", name: "", userName: "" };
    public loading: boolean = false;
    public userFound: boolean = false;
    public usersFound: boolean = false;
    public toSearch: string;
    public users: any[] = new Array<any>();

    private searchSubscrip: Subscription;
    private saveSubscrip: Subscription;

    constructor(private service: UserService, 
                private messageService: MessageService,
                private router: Router,
                private errorHandlerService: ErrorHandlerService) { }

    ngOnDestroy(): void {
        if(this.searchSubscrip) this.searchSubscrip.unsubscribe();
        if(this.saveSubscrip) this.saveSubscrip.unsubscribe();
    }

    save() {
        this.messageService.showLoading();

        var json = {
            email: this.model.userPrincipalName,
            name: this.model.displayName,
            userName: this.model.userName
        }

        this.saveSubscrip = this.service.save(json).subscribe(
            response => {
                this.messageService.closeLoading();
                if(response.messages) this.messageService.showMessages(response.messages);

                setTimeout(() => {
                    this.router.navigate(['/admin/users']);
                }, 500)
            },
            error => this.errorHandlerService.handleErrors(error)
        )
    }
 
    back(){
        this.router.navigate(['/admin/users']);
    }

    select(user){
        this.model = user;
        this.save();
    }

    searchBySurname(){
        this.loading = true;
        this.usersFound = false;
        this.userFound = false;

        this.searchSubscrip = this.service.searchBySurname(this.toSearch).subscribe(
            response => {
                this.loading = false;
                this.users = response.value; 
                
                if(this.users.length > 0){
                    this.usersFound = true;    
                }
                else{
                    this.messageService.showError("ADMIN.USERS.userNotFoundInAd");
                }
            },
            error => {
                this.loading = false;
                this.errorHandlerService.handleErrors(error)
            }
        )
    }

    searchByEmail(){
        this.loading = true;
        this.userFound = false;
        this.usersFound = false;

        this.searchSubscrip = this.service.searchByEmail(this.toSearch).subscribe(
            response => {
                this.loading = false;
                this.userFound = true;

                this.model = response.data;
            },
            error => {
                this.loading = false;
                this.errorHandlerService.handleErrors(error)
            }
        )
    }
}
