import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "app/services/common/message.service";
import { Router } from "@angular/router";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { UserService } from "app/services/admin/user.service";
import { SectorService } from "../../../../services/admin/sector.service";

@Component({
    selector: 'app-sector-add',
    templateUrl: './sector-add.component.html'
})
export class SectorAddComponent implements OnInit, OnDestroy {

    public text: string;
    public responsableId: number = 0;

    public responsables: any[] = new Array();

    public addSubscript: Subscription;
    public responsablesSubscript: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                private sectorService: SectorService,
                private userService: UserService,
                private errorHandlerService: ErrorHandlerService) { }

    ngOnInit(): void {
        this.messageService.showLoading();

        this.responsablesSubscript = this.userService.getOptions().subscribe(response => {
            this.messageService.closeLoading();
            this.responsables = response; 
        }, 
        error => {
            this.errorHandlerService.handleErrors(error);
            this.messageService.closeLoading();
        });
    }

    ngOnDestroy(): void {
        if(this.addSubscript) this.addSubscript.unsubscribe();
        if(this.responsablesSubscript) this.responsablesSubscript.unsubscribe();
    }

    goBack(){
        this.router.navigate(["/admin/sectors"]);  
    }

    save(){
        this.messageService.showLoading();

        this.addSubscript = this.sectorService.add(this.text, this.responsableId).subscribe(response => {
            this.messageService.closeLoading();

            if(response.messages) this.messageService.showMessages(response.messages);
            this.goBack();   
        }, 
        error => { 
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);
        });
    }
}