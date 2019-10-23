import { Component, OnDestroy, OnInit, ViewChild, EventEmitter, Output, Input } from "@angular/core";
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
import { ManagementReportStatus } from "app/models/enums/managementReportStatus";
import { ResourceBillingItem } from "app/models/management-report/resourceBillingItem";
import { GenericOptionService } from "app/services/admin/generic-option.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
// import { Worksheet } from "exceljs";
declare var moment: any;

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

    //region recursos facturados
    getSenioritiesSubscrip: Subscription;
    getPurchaseOrdersSubscrip: Subscription;
    getSubscrip: Subscription;

    seniorities: any[] = new Array();
    employees: any[] = new Array();
    types: any[] = new Array();
    items: ResourceBillingItem[] = new Array();
    month: any;
    billingMonthId: number;
    resourceQuantity: number;
    hitoMonth: number;
    total: number;
    profile: string;
    serviceId: string;
    isLoading: boolean = false;
    //endregion 

    months: any[] = new Array();
    hitos: any[] = new Array();
    currencies: any[] = new Array();
    projects: any[] = new Array();
    totals: any[] = new Array();

    hitosHide: boolean = true;

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
    managementReportStatus: ManagementReportStatus = ManagementReportStatus.CdgPending;
    monthSelectedDisplay: string = "";
    monthSelectedOpportunity: string = "";
    monthSelectedCurrency: string = "";
    editItemMonto = new FormControl('', [Validators.required, Validators.min(1), Validators.max(999999999)]);
    editItemName = new FormControl('', [Validators.required, Validators.maxLength(250)]);
    editItemStartDate = new FormControl(null, [Validators.required]);

    fromMonth: Date = new Date()
    readOnly: boolean = false;

    @Input() manamementReportStartDate: any
    @Input() manamementReportEndDate: any

    @Output() openEvalPropModal: EventEmitter<any> = new EventEmitter();
    @Output() getData: EventEmitter<any> = new EventEmitter();

    // @ViewChild('resourceBillingModal') resourceBillingModal;

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
        "Detalles Hito",
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
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private genericOptionsService: GenericOptionService,
        private projectService: ProjectService,
        private menuService: MenuService) { }

    ngOnInit(): void {
        this.getCurrencies();
        this.getSeniorities();
        this.getEmployees();

        this.types.push({ id: 1, text: "Mes" });
        this.types.push({ id: 2, text: "Horas" });
    }

    ngOnDestroy(): void {
        if (this.getBillingSubscrip) this.getBillingSubscrip.unsubscribe();
        if (this.getCurrenciesSubscrip) this.getCurrenciesSubscrip.unsubscribe();
        if (this.postHitoSubscrip) this.postHitoSubscrip.unsubscribe();
        if (this.updateHitoSubscrip) this.updateHitoSubscrip.unsubscribe();
        if (this.getHitoSubscrip) this.getHitoSubscrip.unsubscribe();
        if (this.updateDataSubscrip) this.updateDataSubscrip.unsubscribe();
        if (this.getSenioritiesSubscrip) this.getSenioritiesSubscrip.unsubscribe();
        if (this.getPurchaseOrdersSubscrip) this.getPurchaseOrdersSubscrip.unsubscribe();
        if (this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    getSeniorities(){
        this.genericOptionsService.controller = "seniority";
        this.getSenioritiesSubscrip = this.genericOptionsService.getOptions().subscribe(response => {
            this.seniorities = response.data;
        },
        () => {});
    }

    getCurrencies() {
        this.getCurrenciesSubscrip = this.utilsService.getCurrencies().subscribe(d => {
            this.currencies = d;
        });
    } 

    getEmployees() {
        this.getCurrenciesSubscrip = this.employeeService.getOptions().subscribe(d => {
            this.employees = d;
        });
    } 

    init(serviceId) {
        this.serviceId = serviceId;
        this.hitos = new Array();

        this.getBillingSubscrip = this.managementReportService.getBilling(serviceId).subscribe(response => {

            // this.resourceBillingModal.getPurchaseOrders(serviceId);

            this.managerId = response.data.managerId;

            this.projects = response.data.projects;
            this.totals = response.data.totals;
            this.months = response.data.monthsHeader;

            this.columnsCount = this.months.length;

            response.data.rows.forEach(row => {

                var hito = { id: "", projectId: "", description: "", projectName: "", currencyId: "", currencyName: "", opportunityNumber: "", date: new Date(), values: [], month: 0 };
                hito.description = row.description;
                hito.id = row.id;
                hito.projectId = row.projectId;
                hito.projectName = row.projectName;
                hito.date = row.date;
                hito.month = row.month;
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
        },
        error => this.messageService.closeLoading());
    }

    sendDataToDetailView() {
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
                opportunityNumber: project.opportunityNumber,
                currencyName: currency.text,
                description: model.name,
                month: model.month,
                values: []
            };

            this.getHito(month, year, hito, model);
        },
        error => this.newHitoModal.resetButtons());
    }

    getHito(month, year, hito, model) {
        var addToReport = false

        this.getHitoSubscrip = this.projectService.getHito(hito.id).subscribe(response => {
            hito.date = response.data.startDate;

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
            
                    monthRow.totalBilling += response.data.baseAmount;

                    if (new Date(monthValue.monthYear) >= new Date(this.manamementReportStartDate)
                        && new Date(monthValue.monthYear) <= new Date(this.manamementReportEndDate)) {
                        addToReport = true
                    }
                }

                hito.values.push(monthValue);
            });

            if (addToReport) {
                this.hitos.push(hito);
            }

            this.sendDataToDetailView();
        });
    }

    updateHito() {
        var descAux = this.editItemName.value;
        var hito = this.hitoSelected

        var hitoMonth = hito.values.find(x => x.value && x.value != null);

        var oldValuePesos = hitoMonth.valuePesos;

        hitoMonth.value = this.editItemMonto.value;

        this.hitoSelected.description = this.editItemName.value;
        this.hitoSelected.date = this.editItemStartDate.value;
        this.hitoSelected.month = this.hitoMonth;
      
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
            projectId: hito.projectId,
            month: hito.month,
            date: hito.date,
            resources: this.items,
            billingMonthId: this.billingMonthId
        }

        this.projectService.updateAmmountHito(json).subscribe(response => {
            hitoMonth.oldValue = hitoMonth.value; 

            var monthValueChange = this.months.find(x => x.month == hitoMonth.month && x.year == hitoMonth.year);

            if(response.data && response.data.quantity){
                monthValueChange.resourceQuantity = response.data.quantity;
            }

            this.editItemValueModal.hide();

            if(hito.date.getFullYear() != hitoMonth.year || (hito.date.getMonth()+1) != hitoMonth.month){
                this.messageService.showLoading();
                this.init(this.serviceId);
            }
            else{
                this.getHitoSubscrip = this.projectService.getHito(hito.id).subscribe(response => {
                    hitoMonth.value = json.ammount;
                    hitoMonth.valuePesos = response.data.baseAmount;
                    hitoMonth.originalValue = response.data.amountOriginal;
                    hitoMonth.originalValuePesos = response.data.baseAmountOriginal;
    
                    var month = this.months.find(x => x.month == hitoMonth.month && x.year == hitoMonth.year);
    
                    if(month != null){
                        month.totalBilling -= oldValuePesos;
                        month.totalBilling += hitoMonth.valuePesos;
                    }
    
                    this.sendDataToDetailView();
                });
            }
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

                var month = this.months.find(x => x.month == hitoMonth.month && x.year == hitoMonth.year);

                if(month != null){
                    month.totalBilling -= hitoMonth.valuePesos;
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

    canDeleteHito(hito, index) {
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

                if (value.originalValuePesos) {
                    totals.totalProvisioned += value.valuePesos;
                }
            }
        });

        totals.provision = totals.totalProvisioned - totals.totalBilling;

        return totals;
    }

    openEditItemModal(value, hito, index) {
        if (this.readOnly) return false;

        var monthValue = this.months.find(x => x.month == value.month && x.year == value.year);
        if(monthValue && monthValue.closed) return;

        var hitoMonth = hito.values.find(x => x.value && x.value != null);

        if (!this.isEnabled(hitoMonth)) return;

        this.editItemValueModal.show();
        this.editItemMonto.setValue(value.value)
        this.editItemName.setValue(hito.description)
        this.editItemStartDate.setValue(moment(hito.date).toDate())
        this.hitoSelected = hito;
        this.monthSelectedDisplay = monthValue.display;
        this.monthSelectedOpportunity = hito.opportunityNumber;
        this.monthSelectedCurrency = hito.currencyName;
        this.hitoMonth = hito.month;

        this.items = [];

        this.billingMonthId = monthValue.billingMonthId;

        this.getDataResources(this.billingMonthId);
    }

    openEditEvalProp(month) {
        if (this.readOnly) return false;

        if(month.closed) return;

        if (this.openEvalPropModal.observers.length > 0) {
            month.type = 1;
            this.openEvalPropModal.emit(month);
        }
    }

    setFromDate(date: Date) {
        this.fromMonth = new Date(date.getFullYear(), date.getMonth() - 2, 1)
    }

    isClosed(date: Date){   
        var item = this.months.find(x => x.month == (date.getMonth()+1) && x.year == date.getFullYear());

        if(item){
            return item.closed;
        }

        return false;
    }

    getId(date: Date){
        var item = this.months.find(x => x.month == (date.getMonth()+1) && x.year == date.getFullYear());

        if(item){
            return item.billingMonthId;
        }

        return 0;
    }

    openEvalPropDifferenceModal(month) {
        if (this.readOnly) return false;

        if(month.closed) return;

        this.monthSelected = month;
        this.updateDataType = ReportBillingUpdateDataType.EvalPropDifference;
        this.evalPropDifference = month.evalPropDifference;
        this.editBillingDataModal.show();
    }

    openCommentsModal(month) {
        if (this.readOnly) return false;

        if(month.closed) return;

        this.monthSelected = month;
        this.updateDataType = ReportBillingUpdateDataType.Comments;
        this.billingComments = month.comments;
        this.editBillingDataModal.show();
    }

    updateBillingData() {
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

            if (this.updateDataType == ReportBillingUpdateDataType.Comments) {
                this.monthSelected.comments = this.billingComments;
            }

            if (this.updateDataType == ReportBillingUpdateDataType.BilledResources) {
                this.monthSelected.resourceQuantity = this.billingResourceQuantity;
            }

            if (this.updateDataType == ReportBillingUpdateDataType.EvalPropDifference) {
                this.monthSelected.evalPropDifference = this.evalPropDifference;
            }
            
            this.monthSelected = null;
            this.sendDataToDetailView()
        },
        error => this.editBillingDataModal.resetButtons());
    }

    seeBillingDetail(month) {
        if (this.readOnly) return false;

        if(month.closed) return;

        var currencies = [];

        this.totals.forEach(total => {

            var totalMonth = total.monthValues.find(x => x.month == month.month && x.year == month.year);

            if (totalMonth) {
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

    // Region facturacion de recursos

    getDataResources(billingMonthId){
        this.isLoading = true;

        this.getSubscrip = this.managementReportService.getResources(billingMonthId, this.hitoSelected.id).subscribe(response => {
            this.isLoading = false;

            if(response.data && response.data.length > 0){
                response.data.forEach(item => {
                    this.items.push(new ResourceBillingItem(item));
                });

                this.resourceQuantity = response.data.length;
                this.calculateTotal();
            }
            else{
                this.resourceQuantity = 0;
            }
        },
        () => this.isLoading = false);
    }

    calculateTotal(){
        this.total = 0;

        this.items.forEach(item => {
            item.subTotal = item.amount * item.quantity;
            
            if(!isNaN(item.subTotal)){
                if(!item.deleted){
                    this.total += item.subTotal;
                }
            }
            else{
                item.subTotal = 0;
            }
        });

        this.editItemMonto.setValue(this.total);
    }

    addItem(){
        this.items.push(new ResourceBillingItem(null));

        this.resourceQuantity = this.items.filter(x => !x.deleted).length;
    }

    monthHourChange(item){
        if(item.monthHour == 1){
            item.quantity = 1;
        }

        this.calculateTotal();
    }

    removeItem(item, index){
        if(item.id > 0){
            item.deleted = true;
        }
        else{
            this.items.splice(index, 1);
        }

        this.resourceQuantity = this.items.filter(x => !x.deleted).length;
        this.calculateTotal();
    }

    // createWorksheet(workbook){
    //     let worksheet: Worksheet = workbook.addWorksheet('Facturación');

    //     this.buildHeader(worksheet);
    //     this.buildHitos(worksheet);

    //     const borderBlack = "FF000000";

    //     var column = worksheet.getColumn(1);
    //     column.eachCell(cell => {
    //         cell.border = { right: { style:'thin', color: { argb: borderBlack} }};
    //     });

    //     var lastColumn = worksheet.getColumn(worksheet.columnCount);
    //     lastColumn.eachCell(cell => {
    //         cell.border = { right: { style:'thin', color: { argb: borderBlack} }};
    //     });

    //     var row1 = worksheet.getRow(1);
    //     var row5 = worksheet.getRow(5);

    //     row1.eachCell(cell => {
    //         cell.style = { font: { bold: true } };
    //         cell.alignment = { horizontal: 'center' };
    //         cell.border = { 
    //             right: { style:'thin', color: { argb: borderBlack} },
    //             bottom: { style:'thin', color: { argb: borderBlack} }
    //         };
    //     });

    //     row5.eachCell(cell => {
    //         if(cell.border && cell.border.right){
    //             cell.border.bottom = { style:'thin', color: { argb: borderBlack} };
    //         }            
    //         else{
    //             cell.border = { bottom: { style:'thin', color: { argb: borderBlack} }};
    //         }
    //     });

    //     var lastRow = worksheet.getRow(worksheet.rowCount);
    //     lastRow.eachCell(cell => {
    //         if(cell.border && cell.border.right){
    //             cell.border.bottom = { style:'thin', color: { argb: borderBlack} };
    //         }            
    //         else{
    //             cell.border = { bottom: { style:'thin', color: { argb: borderBlack} }};
    //         }
    //     });
    // }

    // private buildHitos(worksheet: Worksheet) {
    //     worksheet.addRow(["Hitos"]);
    //     this.hitos.forEach(hito => {
    //         var row = [`${hito.opportunityNumber} - ${hito.description} - ${hito.currencyName}`];
    //         hito.values.forEach(value => {
    //             if (value.value) {
    //                 row.push(value.value);
    //             }
    //             else {
    //                 row.push("");
    //             }
    //         });
    //         worksheet.addRow(row);
    //     });
    // }

    // private buildHeader(worksheet: Worksheet) {
    //     var columns = [];
    //     var monthItem = { header: "Meses", width: 50 };
    //     columns.push(monthItem);

    //     var totalBilling = ["Total Facturacion en Pesos"];
    //     var resourceQuantity = ["Cantidad Recursos facturados"];
    //     var evalprop = ["EVALPROP"];
    //     var billingDiff = ["Diferencias de Facturación"];

    //     this.months.forEach(month => {
    //         columns.push({ header: month.display, width: 15, style: { numFmt: '#,##0.00' }, alignment: { horizontal:'center'} });

    //         totalBilling.push(month.totalBilling || 0);
    //         resourceQuantity.push(month.resourceQuantity || 0);
    //         evalprop.push(month.valueEvalProp || 0);
    //         billingDiff.push(month.evalPropDifference || 0);
    //     });

    //     worksheet.columns = columns;
    //     worksheet.addRow(totalBilling);
    //     worksheet.addRow(resourceQuantity);
    //     worksheet.addRow(evalprop);
    //     worksheet.addRow(billingDiff);
    // }
}