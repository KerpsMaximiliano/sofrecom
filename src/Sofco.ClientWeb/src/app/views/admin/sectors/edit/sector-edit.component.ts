import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "app/services/common/message.service";
import { Router, ActivatedRoute } from "@angular/router";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { UserService } from "app/services/admin/user.service";
import { SectorService } from "../../../../services/admin/sector.service";

@Component({
    selector: 'app-sector-edit',
    templateUrl: './sector-edit.component.html'
})
export class SectorEditComponent implements OnInit, OnDestroy {

    private id: number;
    public text: string;
    public responsableId: number;

    public responsables: any[] = new Array();
    
    public editSubscript: Subscription;
    public paramsSubscrip: Subscription;
    public getSubscrip: Subscription;
    public responsableSubscript: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                private activatedRoute: ActivatedRoute,
                private userService: UserService,
                private sectorService: SectorService,
                private errorHandlerService: ErrorHandlerService) { }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.getDetails();
        });

        this.responsableSubscript = this.userService.getOptions().subscribe(response => {
            this.messageService.closeLoading();
            this.responsables = response;
        }, 
        error => {
            this.errorHandlerService.handleErrors(error);
            this.messageService.closeLoading();
        });
    }

    ngOnDestroy(): void {
        if(this.editSubscript) this.editSubscript.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.responsableSubscript) this.responsableSubscript.unsubscribe();
    }

    getDetails(){
        this.messageService.showLoading();

        this.getSubscrip = this.sectorService.get(this.id).subscribe(response => {
            this.messageService.closeLoading();
            this.text = response.data.text;
            this.responsableId = response.data.responsableId;
        }, 
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);   
        });
    }

    goBack(){
        this.router.navigate(["/admin/sectors"]);  
    }

    update(){
        this.messageService.showLoading();

        this.editSubscript = this.sectorService.edit({ id: this.id, text: this.text, responsableId: this.responsableId }).subscribe(response => {
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