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


    public model: any = { email: "", name: "", userName: "" };
    public loading: boolean = false;
    public userFound: boolean = false;
    public email: string;

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
        this.saveSubscrip = this.service.save(this.model).subscribe(
            response => {
                if(response.messages) this.messageService.showMessages(response.messages);

                setTimeout(() => {
                    this.router.navigate(['/admin/users']);
                }, 1000)
            },
            error => this.errorHandlerService.handleErrors(error)
        )
    }

    back(){
        this.router.navigate(['/admin/users']);
    }

    search(){
        this.loading = true;
        this.userFound = false;

        this.searchSubscrip = this.service.search(this.email).subscribe(
            response => {
                this.loading = false;
                this.userFound = true;
                this.model.name = response.data.displayName;
                this.model.userName = response.data.userName;
                this.model.email = response.data.email;
            },
            error => {
                this.loading = false;
                this.errorHandlerService.handleErrors(error)
            }
        )
    }
}
