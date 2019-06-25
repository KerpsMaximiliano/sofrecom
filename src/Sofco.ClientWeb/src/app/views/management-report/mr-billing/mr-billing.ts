import { Component, OnDestroy, OnInit, ViewChild, EventEmitter, Output } from "@angular/core";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { Subscription } from "rxjs";
import { MenuService } from "app/services/admin/menu.service";
import { NewHito } from "app/models/billing/solfac/newHito";
import { UtilsService } from "app/services/common/utils.service";
import { ProjectService } from "app/services/billing/project.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { MessageService } from "app/services/common/message.service";

import { FormControl, Validators } from "@angular/forms";
import { ReportBillingUpdateDataType } from "app/models/enums/ReportBillingUpdateDataType";

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
    getHitoSubscrip: Subscription;
    updateDataSubscrip: Subscription;
    
    months: any[] = new Array();
    hitos: any[] = new Array();
    currencies: any[] = new Array();
    projects: any[] = new Array();
    totals: any[] = new Array();

    managerId: string;
    managementReportId: number;

    billingDetail: any = {
        exchanges: [],
        currencies: [],
        total: 0
    };

    pendingHitoStatus: string = "Pendiente";
    billedHitoStatus: string = "Facturado";
    cashedHitoStatus: string = "Pagado";

    hito: NewHito = new NewHito();

    hitoSelected: any;
    indexSelected: number = 0
    monthSelectedDisplay: string = "";
    monthSelectedOpportunity: string = "";
    monthSelectedCurrency: string = "";
    editItemMonto = new FormControl('', [Validators.required, Validators.min(1), Validators.max(999999999)]);
    editItemName = new FormControl('', [Validators.required, Validators.maxLength(250)]);

    fromMonth: Date = new Date()
    readOnly: boolean = false;

    @Output() openEvalPropModal: EventEmitter<any> = new EventEmitter();
    @Output() getData: EventEmitter<any> = new EventEmitter();
 
    @ViewChild('newHitoModal') newHitoModal;
    public newHitoModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "billing.project.detail.milestone.splitTitle",
        "newHitoModal",
        true,
        true,
        "ACTIONS.save",
        "ACTIONS.cancel"
    );

    @ViewChild('editItemModal') editItemValueModal;
    public editItemModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Editar Monto",
        "editItemValueModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @ViewChild('editBillingDataModal') editBillingDataModal;
    public editBillingDataModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Editar datos facturación",
        "editBillingDataModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @ViewChild('billingDetailModal') billingDetailModal;
    public billingDetailModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Detalle facturación mensual",
        "billingDetailModal",
        false,
        true,
        "",
        "ACTIONS.close"
    );

    updateDataType: ReportBillingUpdateDataType;
    evalPropDifference: number;
    billingResourceQuantity: number;
    billingComments: string;
    monthSelected: any;

    columnsCount: number = 1;

    constructor(private managementReportService: ManagementReportService,
        private utilsService: UtilsService,
        private messageService: MessageService,
        private projectService: ProjectService,
        private menuService: MenuService) { }

    ngOnInit(): void {
        this.getCurrencies();
    }

    ngOnDestroy(): void {
        if (this.getBillingSubscrip) this.getBillingSubscrip.unsubscribe();
        if (this.getCurrenciesSubscrip) this.getCurrenciesSubscrip.unsubscribe();
        if (this.postHitoSubscrip) this.postHitoSubscrip.unsubscribe();
        if (this.updateHitoSubscrip) this.updateHitoSubscrip.unsubscribe();
        if (this.getHitoSubscrip) this.getHitoSubscrip.unsubscribe();
        if (this.updateDataSubscrip) this.updateDataSubscrip.unsubscribe();
    }

    getCurrencies() {
        this.getCurrenciesSubscrip = this.utilsService.getCurrencies().subscribe(d => {
            this.currencies = d;
        });
    }

    init(serviceId) {
        this.messageService.showLoading();
        this.hitos = new Array();
        
        this.getBillingSubscrip = this.managementReportService.getBilling(serviceId).subscribe(response => {

            this.managerId = response.data.managerId;

            this.projects = response.data.projects;
            this.totals = response.data.totals;
            this.months = response.data.monthsHeader;

            this.columnsCount = this.months.length;

            response.data.rows.forEach(row => {

                var hito = { id: "", projectId: "", description: "", projectName: "", currencyId: "", currencyName: "", opportunityNumber: "", date: new Date(), values: [] };
                hito.description = row.description;
                hito.id = row.id;
                hito.projectId = row.projectId;
                hito.projectName = row.projectName;
                hito.date = row.date;
                hito.currencyId = row.currencyId;
                hito.currencyName = row.currencyName;
                hito.opportunityNumber = row.opportunityNumber;

                this.months.forEach(month => {
                    var monthValue = row.monthValues.find(x => x.month == month.month && x.year == month.year);

                    if (monthValue) {
                        monthValue.oldValue = monthValue.value;
                        hito.values.push(monthValue);
                    }
                    else {
                        hito.values.push({ month: month.month, year: month.year, monthYear: month.monthYear, value: null, oldValue: null });
                    }
                });

                this.hitos.push(hito);
            });

            this.sendDataToDetailView();
            this.messageService.closeLoading();
        });
    }

    sendDataToDetailView(){
        if (this.getData.observers.length > 0) {
            this.getData.emit({
                totals: this.totals,
                months: this.months,
                hitos: this.hitos
            });
        }
    }

    canCreateHito() {
        if (this.readOnly) return false;

        if (!this.menuService.hasFunctionality('SOLFA', 'NEW-HITO')) return false;

        return true;
    }

    openHitoModal() {
        var today = new Date();
        this.hito = new NewHito();
        this.hito.month = today.getMonth() + 1;
        this.hito.managerId = this.managerId;

        this.newHitoModal.show();
    }

    createHito() {
        let model = Object.assign({}, this.hito);

        if (!this.hito.projectId || this.hito.projectId == "") {
            this.messageService.showErrorByFolder('billing/projects', 'required');
            this.newHitoModal.resetButtons();
            return;
        }

        if (!this.hito.moneyId || this.hito.moneyId == 0) {
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

            var month = model.startDate.getMonth() + 1;
            var year = model.startDate.getFullYear();

            var totalCurrency = this.totals.find(x => x.currencyId == model.moneyId);

            if (totalCurrency) {
                var monthValue = totalCurrency.monthValues.find(x => x.month == month && x.year == year);

                if (monthValue) {
                    monthValue.value += model.ammount;
                }
            }

            var hito = {
                id: response.data,
                projectId: project.id,
                projectName: project.text,
                currencyId: model.moneyId,
                description: `${project.opportunityNumber} - ${model.name} - ${currency.text}`,
                values: []
            };

            this.getHito(month, year, hito, model);            
        },
        error => this.newHitoModal.resetButtons());
    }

    getHito(month, year, hito, model){
        this.getHitoSubscrip = this.projectService.getHito(hito.id).subscribe(response => {

            this.months.forEach(monthRow => {
                var monthValue = { 
                                    month: monthRow.month, 
                                    year: monthRow.year, 
                                    monthYear: monthRow.monthYear, 
                                    value: null, 
                                    valuePesos: null, 
                                    oldValue: null, 
                                    status: null, 
                                    originalValue: null, 
                                    originalValuePesos: null  
                                };

                if (monthRow.month == month && monthRow.year == year) {
                    monthValue.value = model.ammount;
                    monthValue.valuePesos = response.data.baseAmount;
                    monthValue.oldValue = model.ammount;
                    monthValue.originalValue = response.data.amountOriginal;
                    monthValue.originalValuePesos = response.data.baseAmountOriginal;
                    monthValue.status = this.pendingHitoStatus;
                }
                hito.values.push(monthValue);
            });

            this.hitos.push(hito);

            this.setFromDate(this.monthSelected)
            this.sendDataToDetailView();
        });
    }

    updateHito() {
        var descAux = this.editItemName.value;
        this.hitoSelected.values[this.indexSelected].value = this.editItemMonto.value;
        this.hitoSelected.description = this.editItemName.value;
        var hito = this.hitoSelected

        var hitoMonth = hito.values.find(x => x.value && x.value != null);

        if (!this.isEnabled(hitoMonth)) return;

        var totalCurrency = this.totals.find(x => x.currencyId == hito.currencyId);

        var monthValue;

        if (totalCurrency) {
            monthValue = totalCurrency.monthValues.find(x => x.month == hitoMonth.month && x.year == hitoMonth.year);

            if (monthValue) {
                monthValue.value -= hitoMonth.oldValue;
                monthValue.value += hitoMonth.value;
            }
        } 

        var json = {
            id: hito.id,
            ammount: hitoMonth.value,
            name: hito.description,
            projectId: hito.projectId
        }

        this.projectService.updateAmmountHito(json).subscribe(response => {
            hitoMonth.oldValue = hitoMonth.value;

            this.getHitoSubscrip = this.projectService.getHito(hito.id).subscribe(response => {
                hitoMonth.value = json.ammount;
                hitoMonth.valuePesos = response.data.baseAmount;
                hitoMonth.originalValue = response.data.amountOriginal;
                hitoMonth.originalValuePesos = response.data.baseAmountOriginal;

                this.sendDataToDetailView();
            });
        },
        error => {
            if (monthValue) {
                monthValue.value -= hitoMonth.value;
                monthValue.value += hitoMonth.oldValue;
            }

            hitoMonth.value = hitoMonth.oldValue;
            this.hitoSelected.description = descAux;

            this.editItemValueModal.hide();
        });

        this.editItemValueModal.hide();
    }

    delete(hito) {
        this.messageService.showConfirm(() => {
            this.messageService.showLoading();

            this.projectService.deleteHito(hito.id, hito.projectId).subscribe(response => {
                this.messageService.closeLoading();

                var hitoMonth = hito.values.find(x => x.value && x.value != null);

                var totalCurrency = this.totals.find(x => x.currencyId == hito.currencyId);

                if (totalCurrency) {
                    var monthValue = totalCurrency.monthValues.find(x => x.month == hitoMonth.month && x.year == hitoMonth.year);

                    if (monthValue) {
                        monthValue.value -= hitoMonth.value;
                    }
                }

                var hitoindex = this.hitos.findIndex(x => x.id == hito.id);

                if (hitoindex != undefined) {
                    this.hitos.splice(hitoindex, 1);
                }

                this.sendDataToDetailView();
            },
            error => this.messageService.closeLoading());
        });
    }

    isEnabled(hito) {
        return (this.menuService.userIsManager || this.menuService.userIsCdg) && hito.status == this.pendingHitoStatus && (!hito.solfacId || hito.solfacId == 0);
    }

    canEditCdg() {
        return this.menuService.userIsCdg;
    }

    canDeleteHito(hito) {
        if (this.readOnly) return false;

        var hitoMonth = hito.values.find(x => x.value && x.value != null);

        return this.isEnabled(hitoMonth);
    }

    resolveHitoLabel(hito) {
        var cssClass;
        if (hito.status == this.pendingHitoStatus && hito.solfacId && hito.solfacId > 0) {
            cssClass = 'input-pending-related';
        }
        else {
            cssClass = `input-${hito.status}`
        }

        if (this.isEnabled(hito)) {
            cssClass += ' cursor-pointer'
        }
        else {
            cssClass += ' not-allowed'
        }
        return cssClass;
    }

    getTotals(month, year) {
        month--;
        var totals = {
            totalProvisioned: 0,
            totalBilling: 0,
            provision: 0
        }

        this.hitos.forEach(hito => {

            var value = hito.values.find(x => x.month == month && x.year == year);

            if (value && value != null && value.valuePesos) {
                if (value.status == this.billedHitoStatus || value.status == this.cashedHitoStatus) {

                    totals.totalBilling += value.valuePesos;
                }

                if(value.originalValuePesos){
                    totals.totalProvisioned += value.originalValuePesos;
                }
            }
        });

        totals.provision = totals.totalProvisioned-totals.totalBilling;

        return totals;
    }

    openEditItemModal(value, hito, index) {
        if (this.readOnly) return false;

        var hitoMonth = hito.values.find(x => x.value && x.value != null);

        if (!this.isEnabled(hitoMonth)) return;

        this.editItemValueModal.show();
        this.editItemMonto.setValue(value.value)
        this.editItemName.setValue(hito.description)
        this.hitoSelected = hito;
        this.indexSelected = index
        this.monthSelectedDisplay = this.months[index].display;
        this.monthSelectedOpportunity = hito.opportunityNumber;
        this.monthSelectedCurrency = hito.currencyName;
    }

    openEditEvalProp(month){
        if (this.readOnly) return false;
        
        if (this.openEvalPropModal.observers.length > 0) {
            month.type = 1;
            this.openEvalPropModal.emit(month);
        }
    }

    setFromDate(date : Date){
        this.fromMonth = new Date(date.getFullYear(), date.getMonth() -2, 1)        
    }

    openResourceQuantity(month){
        this.monthSelected = month;
        this.updateDataType = ReportBillingUpdateDataType.BilledResources;
        this.billingResourceQuantity = month.resourceQuantity;
        this.editBillingDataModal.show();
    }

    openEvalPropDifferenceModal(month){
        this.monthSelected = month;
        this.updateDataType = ReportBillingUpdateDataType.EvalPropDifference;
        this.evalPropDifference = month.evalPropDifference;
        this.editBillingDataModal.show();
    }

    openCommentsModal(month){
        this.monthSelected = month;
        this.updateDataType = ReportBillingUpdateDataType.Comments;
        this.billingComments = month.comments;
        this.editBillingDataModal.show();
    }

    updateBillingData(){ 
        var json = {
            id: this.monthSelected.billingMonthId,
            evalPropDifference: this.evalPropDifference,
            resources: this.billingResourceQuantity,
            comments: this.billingComments,
            type: this.updateDataType,
            managementReportId: this.managementReportId,
            monthYear: this.monthSelected.monthYear
        }
 
        this.updateDataSubscrip = this.managementReportService.updateBillingData(json).subscribe(response => {
            this.editBillingDataModal.hide();

            this.monthSelected.billingMonthId = response.data;

            if(this.updateDataType == ReportBillingUpdateDataType.Comments){
                this.monthSelected.comments = this.billingComments;
            }

            if(this.updateDataType == ReportBillingUpdateDataType.BilledResources){
                this.monthSelected.resourceQuantity = this.billingResourceQuantity;
            }

            if(this.updateDataType == ReportBillingUpdateDataType.EvalPropDifference){
                this.monthSelected.evalPropDifference = this.evalPropDifference;
            }

            this.monthSelected = null;
        },
        error => this.editBillingDataModal.resetButtons());
    }

    seeBillingDetail(month){
        var currencies = [];

        this.totals.forEach(total => {

            var totalMonth = total.monthValues.find(x => x.month == month.month && x.year == month.year);

            if(totalMonth){
                currencies.push({ currencyName: total.currencyName, value: totalMonth.value });
            }
        });

        this.billingDetail = {
            exchanges: month.exchanges,
            currencies: currencies,
            total: month.totalBilling
        };

        this.billingDetailModal.show();
    }
}