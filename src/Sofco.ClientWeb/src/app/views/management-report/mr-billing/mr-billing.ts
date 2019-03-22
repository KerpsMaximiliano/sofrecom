import { Component, OnDestroy, OnInit, Input, ViewChild } from "@angular/core";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { NewHito } from "app/models/billing/solfac/newHito";
import { UtilsService } from "app/services/common/utils.service";
import { ProjectService } from "app/services/billing/project.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { Hito } from "app/models/billing/solfac/hito";

@Component({
    selector: 'management-report-billing',
    templateUrl: './mr-billing.html',
    styleUrls: ['./mr-billing.scss']
})
export class ManagementReportBillingComponent implements OnInit, OnDestroy {
  

    getBillingSubscrip: Subscription;
    getCurrenciesSubscrip: Subscription;
    postHitoSubscrip: Subscription;

    months: any[] = new Array();
    hitos: any[] = new Array();
    currencies: any[] = new Array();
    projects: any[] = new Array();
    
    managerId: string;

    hito: NewHito = new NewHito();

    @ViewChild('newHitoModal') newHitoModal;
    public newHitoModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "billing.project.detail.milestone.splitTitle",
        "newHitoModal",
        true,
        true,
        "ACTIONS.save",
        "ACTIONS.cancel"
    );

    columnsCount: number = 1;

    constructor(private managementReportService: ManagementReportService,
                private utilsService: UtilsService,
                private projectService: ProjectService,
                private menuService: MenuService){}

    ngOnInit(): void {
        this.getCurrencies();
    }

    ngOnDestroy(): void {
        if(this.getBillingSubscrip) this.getBillingSubscrip.unsubscribe();
        if(this.getCurrenciesSubscrip) this.getCurrenciesSubscrip.unsubscribe();
        if(this.postHitoSubscrip) this.postHitoSubscrip.unsubscribe();
    }

    getCurrencies(){
        this.getCurrenciesSubscrip = this.utilsService.getCurrencies().subscribe(d => {
            this.currencies = d;
        });
    }

    init(serviceId){
        this.getBillingSubscrip = this.managementReportService.getBilling(serviceId).subscribe(response => {

            this.managerId = response.data.managerId;

            this.projects = response.data.projects;

            this.months = response.data.monthsHeader.map(item => {
                item.total = 0;
                return item;
            });

            this.columnsCount = this.months.length;

            response.data.rows.forEach(row => {

                var hito = { description: "", values: [] };
                hito.description = row.description;

                this.months.forEach(month => {
                    var monthValue = row.monthValues.find(x => x.month == month.month && x.year == month.year);
    
                    if(monthValue){
                        month.total += monthValue.value;
                        hito.values.push(monthValue);
                    }
                    else {
                        hito.values.push({ month: month.month, year: month.year, value: null  });
                    }
                });

                this.hitos.push(hito);
            });
        });
    }

    canCreateHito(){
        if(!this.menuService.hasFunctionality('SOLFA', 'NEW-HITO')) return false;

        return true;
    }

    openHitoModal(){
        var today = new Date();
        this.hito = new NewHito();
        this.hito.month = today.getMonth()+1;
        this.hito.managerId = this.managerId;

        this.newHitoModal.show();
    }

    createHito(){
        var currency = this.currencies.find(x => x.id == this.hito.moneyId);
        var project = this.projects.find(x => x.id == this.hito.projectId)
        this.hito.opportunityId = project.opportunityId;
        this.hito.moneyId = currency.crmId;

        this.postHitoSubscrip = this.projectService.createNewHito(this.hito).subscribe(() => {
            this.newHitoModal.hide();
        }, 
        error => this.newHitoModal.resetButtons());
    }
}