import { Component, OnInit, OnDestroy } from "@angular/core";
import { Subscription } from "rxjs";
import { SolfacService } from "app/services/billing/solfac.service";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'app-solfac-admin',
    templateUrl: './solfac-admin.component.html'
})
export class SolfacAdminComponent implements OnDestroy {

    solfacId: number;
    accountName: string;
    accountId: string;
    serviceName: string;
    serviceId: string;
    projectName: string;
    projectId: string;
    analytic: string;
    opportunity: string;

    solfacFound: boolean;

    public getSubscript: Subscription;
    public saveSubscript: Subscription;

    constructor(private messageService: MessageService,
                private solfacService: SolfacService) { }

    ngOnDestroy(): void {
        if(this.getSubscript) this.getSubscript.unsubscribe();
        if(this.saveSubscript) this.saveSubscript.unsubscribe();
    }

    search(){
        if(!this.solfacId || this.solfacId == 0) return;

        this.solfacFound = false;

        this.messageService.showLoading();

        this.saveSubscript = this.solfacService.get(this.solfacId).subscribe(response => {
            this.messageService.closeLoading();

            this.accountName = response.businessName;
            this.accountId = response.customerId;
            this.serviceName = response.serviceName;
            this.serviceId = response.serviceId;
            this.projectName = response.project;
            this.projectId = response.projectId;
            this.analytic = response.imputationNumber1;
            this.opportunity = response.opportunityNumber;

            this.solfacFound = true;
        }, 
        error => { 
            this.messageService.closeLoading();
        });
    }

    save(){
        var json = {
            accountName: this.accountName,
            accountId: this.accountId,
            serviceName: this.serviceName,
            serviceId: this.serviceId,
            projectName: this.projectName,
            projectId: this.projectId,
            analytic: this.analytic,
            opportunity: this.opportunity
        };

        this.messageService.showLoading();

        this.saveSubscript = this.solfacService.adminUpdate(this.solfacId, json).subscribe(response => {
            this.messageService.closeLoading();
        }, 
        error => { 
            this.messageService.closeLoading();
        });
    }
}