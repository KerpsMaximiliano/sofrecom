import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { I18nService } from "app/services/common/i18n.service";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";
import { ManagementReportService } from "app/services/management-report/management-report.service";
import { ActivatedRoute } from "@angular/router";
import { ManagementReportDetailComponent } from "../detail/mr-detail"
import { EmployeeService } from "app/services/allocation-management/employee.service"
import { category } from "app/models/management-report/category";
import { ManagementReportStaffService } from "app/services/management-report/management-report-staff.service";
import { Workbook, Worksheet } from "exceljs";
import * as fs from 'file-saver';

@Component({
    selector: 'cost-detail-month',
    templateUrl: './cost-detail-month.html',
    styleUrls: ['./cost-detail-month.scss']
})
export class CostDetailMonthComponent implements OnInit, OnDestroy {

    updateCostSubscrip: Subscription;
    getContratedSuscrip: Subscription;
    //getOtherSuscrip: Subscription;
    deleteContractedSuscrip: Subscription;
    deleteOtherSuscrip: Subscription;
    paramsSubscrip: Subscription;
    getEmployeesSubscrip: Subscription

    totalProvisioned: number = 0;
    totalProvisionedAux: number;
    totalCosts: number = 0;
    totalBilling: number = 0;
    totalBillingAux: number;
    provision: number = 0;
    provisionAux: number;
    totalChargesPercentage: number = 0;
    categoriesSubTotal: number = 0;
    resourcesSubTotal: number = 0;
    resourcesSalarySubTotal: number = 0;
    resourcesChargesSubTotal: number = 0;
    contractedsSubTotal: number = 0;
    contractedsHonorarySubTotal: number = 0;
    contractedsInsuranceSubTotal: number = 0;

    totalProvisionedEditabled: boolean = false;
    totalBillingEditabled: boolean = false;
    provisionEditabled: boolean = false;

    resources: any[] = new Array();
    expenses: any[] = new Array();
    users: any[] = new Array()

    socialCharges: any[] = new Array();

    serviceId: string;
    AnalyticId: any;
    fundedResources: any[] = new Array();
    // otherResources: any[] = new Array();
    // otherSelected: any;
    managementReportId: number;
    contracted: any[] = new Array();
    monthYear: Date;
    canSave: boolean = false;
    userSelected: any
    hasCostProfile: boolean = false

    getCategoriesSuscrip: Subscription;
    categories: category[] = new Array()
    categorySelected: category;
    subcategories: any[] = new Array();
    subcategorySelected: any;
    subCategoriesData: any[] = new Array()

    isReadOnly: boolean
    
    @ViewChild('costDetailMonthModal') costDetailMonthModal;
    public costDetailMonthModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "managementReport.costDetailMonth",
        "costDetailMonthModal",
        true,
        true,
        "ACTIONS.save",
        "ACTIONS.cancel"
    );

    constructor(public i18nService: I18nService,
        private messageService: MessageService,
        private managementReportService: ManagementReportService,
        private activatedRoute: ActivatedRoute,
        private managementReport: ManagementReportDetailComponent,
        private managementReportStaffService: ManagementReportStaffService,
        private employeeService: EmployeeService
    ) { }

    ngOnInit(): void {

        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.serviceId = params['serviceId'];
        });

        // this.getOtherSuscrip = this.managementReportService.getOtherResources().subscribe(response => {
        //     this.otherResources = response.data;

        //     if (this.otherResources.length > 0) {
        //         this.otherSelected = this.otherResources[0];
        //     }
        // });

        this.getCategories()
        this.getUsers()
    }

    ngOnDestroy(): void {
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.updateCostSubscrip) this.updateCostSubscrip.unsubscribe();
        if (this.deleteContractedSuscrip) this.deleteContractedSuscrip.unsubscribe();
        if (this.getContratedSuscrip) this.getContratedSuscrip.unsubscribe();
        if (this.getCategoriesSuscrip) this.getCategoriesSuscrip.unsubscribe()
        if (this.deleteOtherSuscrip) this.deleteOtherSuscrip.unsubscribe();
        if (this.getEmployeesSubscrip) this.getEmployeesSubscrip.unsubscribe()
    }

    totalProvisionedChanged() {
        this.totalProvisionedAux = this.totalProvisioned;
    }

    totalBillingChanged() {
        this.totalBillingAux = this.totalBilling;
    }

    provisionChanged() {
        this.provisionAux = this.provision;
    }

    addExpense() {

        var resource = {
            id: 0,
            subcategoryId: this.subcategorySelected.id,
            subcategoryName: this.subcategorySelected.name,
            categoryName: this.categorySelected.name,
            value: 0,
            description: ""
        }

        this.expenses.push(resource)
    }

    addContracted() {
        this.canSave = false;
        this.contracted.push({ contractedId: 0, name: "", honorary: 0, insurance: 0, total: 0, monthYear: this.monthYear })
    }

    contractedChange(hire) {
        
        hire.total = hire.honorary + hire.insurance;
        this.calculateTotalCosts();
    }

    deleteContracted(index, item) {

        if (item.contractedId > 0) {
            this.deleteContractedSuscrip = this.managementReportService.deleteContracted(item.contractedId).subscribe(() => {
                this.contracted.splice(index, 1)
            },
                () => {
                });
        }
        else {
            this.contracted.splice(index, 1)
        }

        if (this.contracted.length == 0) {
            this.canSave = true;
        }
    }

    deleteResource(index, item) {

        //Si el item no esta en base de datos solo lo borro del array
        if (item.id == 0) {
            this.resources.splice(index, 1);
        }
    }

    deleteExpense(index, item) {

        //Si el item no esta en base de datos solo lo borro del array
        if (item.id == 0) {
            this.expenses.splice(index, 1);
        }
        else {
            //Si esta en base de datos borro el registio
            this.deleteContractedSuscrip = this.managementReportService.deleteOtherResources(item.id).subscribe(() => {
                this.expenses.splice(index, 1);
            },
                () => {
                });
        }

        this.calculateTotalCosts();
    }

    open(data, readOnly) {
        this.messageService.showLoading()
        this.expenses = [];
       
        this.costDetailMonthModal.otherTitle = `${data.monthDesc} ${data.year}`
 
        this.isReadOnly = readOnly;
        this.AnalyticId = data.AnalyticId;
        
        this.resources = data.resources.employees.filter( x=> x.hasAlocation == true || x.salary > 0 || x.charges > 0)
        this.totalBilling = data.totals.totalBilling;
        this.totalProvisioned = data.totals.totalProvisioned;
        this.provision = data.totals.provision;

        var employeeIds = this.resources.map(x => x.employeeId);

        this.getContratedSuscrip = this.managementReportService.getCostDetailMonth(this.serviceId, data.month, data.year, employeeIds).subscribe(response => {

            this.managementReportId = response.data.managementReportId;
            this.monthYear = response.data.monthYear
            this.contracted = response.data.contracted;
            this.expenses = response.data.otherResources;
            this.hasCostProfile = response.data.hasCostProfile
            this.socialCharges = response.data.socialCharges;

            if (response.data.totalBilling && response.data.totalBilling != null) this.totalBilling = response.data.totalBilling;
            if (response.data.provision && response.data.provision != null) this.provision = response.data.provision;
            if (response.data.totalProvisioned && response.data.totalProvisioned != null) this.totalProvisioned = response.data.totalProvisioned;

            this.calculateTotalCosts();

            if(this.hasCostProfile){
                this.messageService.showWarning('managementReport.existProfiles')
            }

            this.messageService.closeLoading();
            this.costDetailMonthModal.show();
        },
            () => {
                this.messageService.closeLoading();
                this.costDetailMonthModal.hide();
            });
    }

    save() {

        if(this.hasCostProfile){
            this.messageService.showError('managementReport.existProfiles')
            this.costDetailMonthModal.resetButtons()
            return
        }

        this.messageService.showLoading();

        var model = {
            AnalyticId: 0,
            ManagementReportId: 0,
            MonthYear: new Date(),
            IsReal: true,
            Employees: [],
            OtherResources: [],
            Contracted: [],
            totalBilling: this.totalBillingAux != null ? this.totalBillingAux : null,
            totalProvisioned: this.totalProvisionedAux != null ? this.totalProvisionedAux : null,
            provision: this.provisionAux != null ? this.provisionAux : null,
        }

        model.AnalyticId = this.AnalyticId
        model.ManagementReportId = this.managementReportId
        model.MonthYear = this.monthYear
        model.Employees = this.resources
        model.OtherResources = this.expenses
        model.Contracted = this.contracted;

        this.updateCostSubscrip = this.managementReportService.PostCostDetailMonth(this.serviceId, model).subscribe(() => {
            this.messageService.closeLoading();
            this.costDetailMonthModal.hide();
            this.managementReport.updateDetailCost()
        },
        () => {
            this.messageService.closeLoading();
            this.costDetailMonthModal.resetButtons();
        });
    }

    subResourceChange(subResource) {
        subResource.total = subResource.salary + subResource.insurance;

        this.calculateTotalCosts();
    }

    resourceChange(resource) {
        resource.modified = true
        resource.total = resource.salary + resource.charges;

        if(resource.salary > 0){
            resource.chargesPercentage = (resource.charges/resource.salary)*100;
        }
        else{
            resource.chargesPercentage = 0;
        }

        this.calculateTotalCosts();
    }

    calculateTotalCosts() {
        this.totalCosts = 0;
        this.categoriesSubTotal = 0;
        this.resourcesSubTotal = 0;
        this.contractedsSubTotal = 0;
        this.contractedsHonorarySubTotal = 0;
        this.contractedsInsuranceSubTotal = 0;
        this.resourcesSalarySubTotal = 0;
        this.resourcesChargesSubTotal = 0;
        var totalCharges = 0;
        var totalSalary= 0;

        this.expenses.forEach(element => {
            this.totalCosts += element.value;
            this.categoriesSubTotal += element.value;
        });

        this.resources.forEach(element => {
            this.totalCosts += element.total;
            this.resourcesSubTotal += element.total;
            this.resourcesSalarySubTotal += element.salary;
            this.resourcesChargesSubTotal += element.charges;
            totalCharges += element.charges;
            totalSalary += element.salary;
        });

        this.contracted.forEach(element => {
            this.totalCosts += element.total;
            this.contractedsSubTotal += element.total;
            this.contractedsHonorarySubTotal += element.honorary;
            this.contractedsInsuranceSubTotal += element.insurance;
        });
     
        if(totalSalary > 0){
            this.totalChargesPercentage = (totalCharges/totalSalary)*100;
        }
    }

    getUsers() {
        this.getEmployeesSubscrip = this.employeeService.getListItems().subscribe(data => {
            this.users = data;
        });
    }

    addEmployee() {

        if (this.userSelected) {
            var existingEmployee = this.resources.find(e => e.employeeId === this.userSelected.id)
            if (!existingEmployee) {
                var costEmployee = {
                    id: 0,
                    costDetailId: 0,
                    employeeId: this.userSelected.id,
                    userId: this.userSelected.userId,
                    name: `${this.userSelected.text.toUpperCase()} - ${this.userSelected.employeeNumber}`,
                    salary: 0,
                    charges: 0,
                    chargesPercentage: 0,
                    total: 0,
                    hasAlocation: false,
                    new: true
                }

                this.resources.push(costEmployee)
            }
            else {
                this.messageService.showError("managementReport.existingEmployee")
            }
        }
        else {
            this.messageService.showError("managementReport.userRequired")
        }
    }

    getCategories() {
        this.getCategoriesSuscrip = this.managementReportStaffService.getCostDetailCategories().subscribe(
            data => {                
                this.categories = data.data;

                if(this.categories.length > 0){
                    this.categorySelected = this.categories[0]
                    this.categoryChange()
                }
            },
            () => { });
    }

    categoryChange() {        
        this.subcategorySelected = {}
        this.subcategories = new Array()
        this.subcategories = this.categorySelected.subcategories

        if(this.subcategories.length > 0){
            this.subcategorySelected = this.subcategories[0]
        }
    }

    private setTitleStyles(cell){
        cell.style = { font: { bold: true } };
        cell.alignment = { horizontal: 'center' };
    }

    private setCellNumber(cell){
        cell.numFmt = '#,##0.00';
    }

    createWorksheet(){
        let workbook = new Workbook();

        this.buildFirstSheet(workbook);
        this.buildSecodSheet(workbook);

        var title = `Detalle mensual ${this.monthYear}.xlsx`

        workbook.xlsx.writeBuffer().then((data) => {
            let blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
            fs.saveAs(blob, title);
        });
    }

    private buildFirstSheet(workbook: Workbook) {
        let worksheet: Worksheet = workbook.addWorksheet('Detalle mensual');
        let columns = [{ width: 50 },
        { width: 25 },
        { width: 25 },
        { width: 20 },
        { width: 15 }];
        worksheet.columns = columns;
        var row1 = ["REAL", this.totalProvisioned, "FACTURACIÓN", "", this.totalBilling];
        var row1Added = worksheet.addRow(row1);
        this.setTitleStyles(row1Added.getCell(1));
        this.setTitleStyles(row1Added.getCell(3));
        this.setCellNumber(row1Added.getCell(2));
        this.setCellNumber(row1Added.getCell(5));
        this.drawBorders(row1Added, 'bottom');
        this.drawCellBorder(row1Added.getCell(2), 'right');
        this.drawCellBorder(row1Added.getCell(4), 'right');
        var row2 = ["TOTAL COSTOS", this.totalCosts, "PROVISION", "", this.provision];
        var row2Added = worksheet.addRow(row2);
        this.setTitleStyles(row2Added.getCell(1));
        this.setTitleStyles(row2Added.getCell(3));
        this.setCellNumber(row2Added.getCell(2));
        this.setCellNumber(row2Added.getCell(5));
        this.drawBorders(row2Added, 'bottom');
        this.drawCellBorder(row2Added.getCell(2), 'right');
        this.drawCellBorder(row2Added.getCell(4), 'right');
        worksheet.mergeCells(`C1:D1`);
        worksheet.mergeCells(`C2:D2`);
        if (this.resources && this.resources.length > 0) {
            worksheet.addRow([]);
            var headerTable = ["Costos RD + P", "Bruto", "Cargas", "Total General", "% Cargas"];
            var headerRow = worksheet.addRow(headerTable);
            headerRow.eachCell(cell => {
                this.setTitleStyles(cell);
            });
            this.drawBorders(headerRow, 'bottom');
            this.drawBorders(headerRow, 'top');
            this.resources.forEach(resource => {
                var rowToAdd = [resource.name, resource.salary, resource.charges, resource.total, resource.chargesPercentage];
                var rowAdded = worksheet.addRow(rowToAdd);
                var first = true;
                rowAdded.eachCell(cell => {
                    if (!first) {
                        this.setCellNumber(cell);
                    }
                    first = false;
                });
            });
            var subtotal = ["Sub Total", this.resourcesSalarySubTotal, this.resourcesChargesSubTotal, this.resourcesSubTotal, this.totalChargesPercentage];
            var subTotalAdded = worksheet.addRow(subtotal);
            this.setTitleStyles(subTotalAdded.getCell(1));
            this.drawBorders(subTotalAdded, 'bottom');
            var first = true;
            subTotalAdded.eachCell(cell => {
                if (!first) {
                    this.setCellNumber(cell);
                }
                first = false;
            });
        }
        if (this.contracted && this.contracted.length > 0) {
            worksheet.addRow([]);
            var headerTable = ["Costos contratados + SubContratados", "Honorarios", "Seguro", "Total General"];
            var headerRow = worksheet.addRow(headerTable);
            headerRow.eachCell(cell => {
                this.setTitleStyles(cell);
            });
            this.drawBorders(headerRow, 'bottom');
            this.drawBorders(headerRow, 'top');
            this.contracted.forEach(resource => {
                var rowToAdd = [resource.name, resource.honorary, resource.insurance, resource.total];
                var rowAdded = worksheet.addRow(rowToAdd);
                var first = true;
                rowAdded.eachCell(cell => {
                    if (!first) {
                        this.setCellNumber(cell);
                    }
                    first = false;
                });
            });
            var subtotal = ["Sub Total", this.contractedsHonorarySubTotal, this.contractedsInsuranceSubTotal, this.contractedsSubTotal];
            var subTotalAdded = worksheet.addRow(subtotal);
            this.setTitleStyles(subTotalAdded.getCell(1));
            this.drawBorders(subTotalAdded, 'bottom');
            var first = true;
            subTotalAdded.eachCell(cell => {
                if (!first) {
                    this.setCellNumber(cell);
                }
                first = false;
            });
        }
        if (this.expenses && this.expenses.length > 0) {
            worksheet.addRow([]);
            var headerTable = ["Categoria", "SubCategoria", "Descripcion", "Total General"];
            var headerRow = worksheet.addRow(headerTable);
            headerRow.eachCell(cell => {
                this.setTitleStyles(cell);
            });
            this.drawBorders(headerRow, 'bottom');
            this.drawBorders(headerRow, 'top');
            this.expenses.forEach(expense => {
                var rowToAdd = [expense.categoryName, expense.subcategoryName, expense.description, expense.value];
                var rowAdded = worksheet.addRow(rowToAdd);
                this.setCellNumber(rowAdded.getCell(4));
            });
            var subtotal = ["", "", "Sub Total", this.categoriesSubTotal];
            var subtotalAdded = worksheet.addRow(subtotal);
            this.setCellNumber(subtotalAdded.getCell(4));
            this.setTitleStyles(subtotalAdded.getCell(3));
            this.drawCellBorder(subtotalAdded.getCell(3), 'bottom');
            this.drawCellBorder(subtotalAdded.getCell(4), 'bottom');
        }
    }

    private buildSecodSheet(workbook: Workbook) {
        let worksheet: Worksheet = workbook.addWorksheet('Detalle de cargas');

        let columns = [{ header: "Recurso", width: 50 },
                        { header: "Legajo", width: 10 },
                        { header: "Cuenta", width: 37 },
                        { header: "Número", width: 10 },
                        { header: "Monto", width: 20, style: { numFmt: '#,##0.00' } },
                        { header: "Año", width: 7 },
                        { header: "Mes", width: 7 }];

        worksheet.columns = columns;

        const borderBlack = "FF000000";
        var row1 = worksheet.getRow(1);

        row1.eachCell(cell => {
            cell.style = { font: { bold: true } };
            cell.alignment = { horizontal: 'center' };
            cell.border = { bottom: { style: 'thin', color: { argb: borderBlack } } };
        });

        if (this.socialCharges && this.socialCharges.length > 0) {
            let employeeNumber = "";

            this.socialCharges.forEach(x => {
                if (employeeNumber && employeeNumber != "" && employeeNumber != x.employeeNumber) {
                    employeeNumber = x.employeeNumber;
                    var lastRow = worksheet.getRow(worksheet.rowCount);
                    this.drawBorders(lastRow, 'bottom')
                }

                var item = [];

                item.push(x.employee);
                item.push(parseInt(x.employeeNumber));
                item.push(x.accountName);
                item.push(x.accountNumber);
                item.push(parseFloat(x.value));
                item.push(x.year);
                item.push(x.month);

                worksheet.addRow(item);

                employeeNumber = x.employeeNumber;
            });
        }
        var lastRow = worksheet.getRow(worksheet.rowCount);

        lastRow.eachCell(cell => {
            cell.border = {
                bottom: { style: 'thin', color: { argb: borderBlack } },
            };
        });
    }

    private drawBorders(row, position) {
        row.eachCell((cell, colNumber) => {
           this.drawCellBorder(cell, position);
        });
    }

    private drawCellBorder(cell, position) {
        const borderBlack = "FF000000";

        if (cell.border) {
            if (cell.border[position]) {
                cell.border[position].style = 'thin';
                cell.border[position].color.argb = borderBlack;
            }
            else {
                cell.border[position] = { style: 'thin', color: { argb: borderBlack } };
            }
        }
        else {
            cell.border = {};
            cell.border[`${position}`] = {};
            cell.border[position] = { style: 'thin', color: { argb: borderBlack } };
        }
    }
}