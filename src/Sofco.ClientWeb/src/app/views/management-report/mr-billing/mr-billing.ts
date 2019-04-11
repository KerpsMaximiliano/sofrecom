import { Component, OnDestroy, OnInit, Input, ViewChild } from "@angular/core";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { NewHito } from "app/models/billing/solfac/newHito";
import { UtilsService } from "app/services/common/utils.service";
import { ProjectService } from "app/services/billing/project.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";
import * as moment from 'moment';
import { I18nService } from "app/services/common/i18n.service";

@Component({
    selector: 'management-report-billing',
    templateUrl: './mr-billing.html',
    styleUrls: ['./mr-billing.scss']
})
export class ManagementReportBillingComponent implements OnInit, OnDestroy {
  

    getBillingSubscrip: Subscription;
    getCurrenciesSubscrip: Subscription;
    postHitoSubscrip: Subscription;
    updateHitoSubscrip: Subscription;

    months: any[] = new Array();
    hitos: any[] = new Array();
    currencies: any[] = new Array();
    projects: any[] = new Array();
    totals: any[] = new Array();
    
    managerId: string;

    pendingHitoStatus:string = "Pendiente";
    billedHitoStatus:string = "Facturado";
    cashedHitoStatus:string = "Pagado";

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
                private messageService: MessageService,
                private projectService: ProjectService,
                private i18nService: I18nService,
                private menuService: MenuService){}

    ngOnInit(): void {
        this.getCurrencies();
    }

    ngOnDestroy(): void {
        if(this.getBillingSubscrip) this.getBillingSubscrip.unsubscribe();
        if(this.getCurrenciesSubscrip) this.getCurrenciesSubscrip.unsubscribe();
        if(this.postHitoSubscrip) this.postHitoSubscrip.unsubscribe();
        if(this.updateHitoSubscrip) this.updateHitoSubscrip.unsubscribe();
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
            this.totals = response.data.totals;
            this.months = response.data.monthsHeader;

            this.columnsCount = this.months.length;

            response.data.rows.forEach(row => {

                var hito = { id: "", projectId: "", description: "", projectName: "", currencyId: "", date: new Date(), values: [] };
                hito.description = row.description;
                hito.id = row.id;
                hito.projectId = row.projectId;
                hito.projectName = row.projectName;
                hito.date = row.date;
                hito.currencyId = row.currencyId;

                this.months.forEach(month => {
                    var monthValue = row.monthValues.find(x => x.month == month.month && x.year == month.year);
    
                    if(monthValue){
                        monthValue.oldValue = monthValue.value;
                        hito.values.push(monthValue);
                    }
                    else {
                        hito.values.push({ month: month.month, year: month.year, value: null, oldValue: null  });
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
        let model = Object.assign({}, this.hito);

        if(!this.hito.projectId || this.hito.projectId == ""){
            this.messageService.showErrorByFolder('billing/projects', 'required');
            this.newHitoModal.resetButtons();
            return;
        }

        if(!this.hito.moneyId || this.hito.moneyId == 0){
            this.messageService.showErrorByFolder('billing/solfac', 'currencyRequired');
            this.newHitoModal.resetButtons();
            return;
        }

        var currency = this.currencies.find(x => x.id == this.hito.moneyId);
        var project = this.projects.find(x => x.id == this.hito.projectId)

        model.opportunityId = project.opportunityId;
        model.moneyId = currency.crmId;

        this.postHitoSubscrip = this.projectService.createNewHito(model).subscribe(response => {
            this.newHitoModal.hide();

            let hitosAux = [];
            var newInserted = false;

            var month = model.startDate.getMonth()+1;
            var year = model.startDate.getFullYear();

            var totalCurrency = this.totals.find(x => x.currencyId == model.moneyId);

            if(totalCurrency){
                var monthValue = totalCurrency.monthValues.find(x => x.month == month && x.year == year);

                if(monthValue){
                    monthValue.value += model.ammount;
                }
            }

            this.hitos.forEach(element => {

                if(!newInserted && moment(element.date).toDate() > model.startDate){
                    newInserted = true;

                    var hito = { id: response.data, 
                        projectId: this.hito.projectId, 
                        projectName: element.projectName, 
                        currencyId: model.moneyId, 
                        description: element.projectName + " - " + model.name + " - " + currency.text, 
                        values: [] };

                    this.months.forEach(monthRow => {
                        var monthValue = { month: monthRow.month, year: monthRow.year, value: null, oldValue: null, status: null  };

                        if(monthRow.month == month && monthRow.year == year){
                            monthValue.value = model.ammount;
                            monthValue.oldValue = model.ammount;
                            monthValue.status = this.pendingHitoStatus;
                        }

                        hito.values.push(monthValue);
                    });

                    hitosAux.push(hito);

                }

                hitosAux.push(element);
            });

            this.hitos = hitosAux;
        }, 
        error => this.newHitoModal.resetButtons());
    }

    updateHito(hito){
        var hitoMonth = hito.values.find(x => x.value && x.value != null);

        if(!this.isEnabled(hitoMonth)) return;

        if(hitoMonth.value > 0 && hitoMonth.value != hitoMonth.oldValue){
            var totalCurrency = this.totals.find(x => x.currencyId == hito.currencyId);
    
            var monthValue;

            if(totalCurrency){
                monthValue = totalCurrency.monthValues.find(x => x.month == hitoMonth.month && x.year == hitoMonth.year);

                if(monthValue){
                    monthValue.value -= hitoMonth.oldValue;
                    monthValue.value += hitoMonth.value;
                }
            }

            var json = {
                id: hito.id,
                ammount: hitoMonth.value,
                projectId: hito.projectId
            }
    
            this.projectService.updateAmmountHito(json).subscribe(response => {
                hitoMonth.oldValue = hitoMonth.value;
            }, 
            error => {
                if(monthValue){
                    monthValue.value -= hitoMonth.value;
                    monthValue.value += hitoMonth.oldValue;
                }

                hitoMonth.value = hitoMonth.oldValue;
            });
        }
        else{
            hitoMonth.value = hitoMonth.oldValue;
        }
    }

    delete(hito){
        this.messageService.showConfirm(() => {
            this.messageService.showLoading();

            this.projectService.deleteHito(hito.id, hito.projectId).subscribe(response => {
                this.messageService.closeLoading();

                var hitoMonth = hito.values.find(x => x.value && x.value != null);

                var totalCurrency = this.totals.find(x => x.currencyId == hito.currencyId);
    
                if(totalCurrency){
                    var monthValue = totalCurrency.monthValues.find(x => x.month == hitoMonth.month && x.year == hitoMonth.year);
    
                    if(monthValue){
                        monthValue.value -= hitoMonth.value;
                    }
                }

                var hitoindex = this.hitos.findIndex(x => x.id == hito.id);

                if(hitoindex) this.hitos.splice(hitoindex, 1);
            }, 
            error => this.messageService.closeLoading());
        });
    }

    isEnabled(hito){
        return this.menuService.userIsManager && hito.status == this.pendingHitoStatus && (!hito.solfacId || hito.solfacId == 0);
    }

    canDeleteHito(hito){
        var hitoMonth = hito.values.find(x => x.value && x.value != null);

        return this.isEnabled(hitoMonth);
    }

    resolveHitoLabel(hito){ 
        if(hito.status == this.pendingHitoStatus && hito.solfacId && hito.solfacId > 0){
            return 'input-pending-related';
        }
        
        return `input-${hito.status}`
    }

    getTotals(month, year){
        var totals = {
            totalProvisioned: 0,
            totalBilling: 0
        }

        this.hitos.forEach(hito => {

            var value = hito.values.find(x => x.month == month && x.year == year);

            if(value && value != null){
                if(value.status == this.billedHitoStatus || value.status == this.cashedHitoStatus){

                    if(value.valuePesos > 0){
                        totals.totalBilling += value.valuePesos;
                    }
                    else{
                        totals.totalBilling += value.value;
                    }
                }

                totals.totalProvisioned += value.value;
            }
        });

        return totals;
    }
}