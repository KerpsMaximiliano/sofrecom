import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { I18nService } from "app/services/common/i18n.service";
import { MessageService } from "app/services/common/message.service";
import { Subscription } from "rxjs";
import { EmployeeService } from "app/services/allocation-management/employee.service"
import { ManagementReportDetailStaffComponent } from "../detail/detail-staff";
import { ManagementReportStaffService } from "app/services/management-report/management-report-staff.service";
import { category } from "app/models/management-report/category";
import { Workbook, Worksheet } from "exceljs";
import * as fs from 'file-saver';

@Component({
    selector: 'cost-detail-month-staff',
    templateUrl: './cost-month-staff.html',
    styleUrls: ['./cost-month-staff.scss']
})
export class CostDetailMonthStaffComponent implements OnInit, OnDestroy {

    updateCostSubscrip: Subscription;
    getContratedSuscrip: Subscription;
    deleteContractedSuscrip: Subscription;
    deleteOtherSuscrip: Subscription;
    paramsSubscrip: Subscription;
    getEmployeesSubscrip: Subscription

    getCategoriesSuscrip: Subscription;
    categories: category[] = new Array()
    categorySelected: category;
    subcategories: any[] = new Array();
    subcategorySelected: any;
    subCategoriesData: any[] = new Array();
    socialCharges: any[] = new Array();

    totalCosts: number = 0;
    totalProvisioned: number = 0;
    totalProvisionedAux: number = 0;
    totalChargesPercentage: number = 0;
    resourcesSubTotal: number = 0;
    categoriesSubTotal: number = 0;
    resourcesSalarySubTotal: number = 0;
    resourcesChargesSubTotal: number = 0;

    totalProvisionedEditabled: boolean = false;

    resources: any[] = new Array();
    // expenses: any[] = new Array();
    users: any[] = new Array()

    AnalyticId: any;
    fundedResources: any[] = new Array();
    otherResources: any[] = new Array();
    otherSelected: any;
    managementReportId: number;
    //contracted: any[] = new Array();
    monthYear: Date;
    canSave: boolean = true;
    userSelected: any

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
        private managementReportStaffService: ManagementReportStaffService,
        private managementReport: ManagementReportDetailStaffComponent,
        private employeeService: EmployeeService
    ) { }

    ngOnInit(): void {
        this.getUsers();
        this.getCategories();
    }

    ngOnDestroy(): void {
        if (this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if (this.updateCostSubscrip) this.updateCostSubscrip.unsubscribe();
        if (this.getContratedSuscrip) this.getContratedSuscrip.unsubscribe();
        if (this.deleteOtherSuscrip) this.deleteOtherSuscrip.unsubscribe();
        if (this.getEmployeesSubscrip) this.getEmployeesSubscrip.unsubscribe()
        if (this.getCategoriesSuscrip) this.getCategoriesSuscrip.unsubscribe()
    }

    totalProvisionedChanged() {
        this.totalProvisionedAux = this.totalProvisioned;
    }

    deleteResource(index, item) {
        //Si el item no esta en base de datos solo lo borro del array
        if (item.id == 0) {
            this.resources.splice(index, 1);
        }
    }

    open(data, readOnly) {
        this.messageService.showLoading()
        //this.expenses = [];

        this.managementReportId = data.managementReportId;
        this.costDetailMonthModal.otherTitle = `${data.monthDesc} ${data.year}`
      
        this.isReadOnly = readOnly;
        this.AnalyticId = data.AnalyticId;

        this.getContratedSuscrip = this.managementReportStaffService.getCostDetailMonth(this.managementReportId, data.month, data.year).subscribe(response => {
            this.monthYear = response.data.monthYear
            this.resources = response.data.employees;
            this.subCategoriesData = response.data.subcategories;
            this.totalProvisioned = response.data.totalProvisioned;
            this.socialCharges = response.data.socialCharges;

            this.calculateTotalCosts();

            this.messageService.closeLoading();
            this.costDetailMonthModal.show();
        },
            () => {
                this.messageService.closeLoading();
                this.costDetailMonthModal.hide();
            });
    }

    save() {
        var model = {
            AnalyticId: 0,
            ManagementReportId: 0,
            MonthYear: new Date(),
            Employees: [],
            OtherResources: [],
            Contracted: [],
            totalBilling: 0,
            totalProvisioned: this.totalProvisionedAux != null ? this.totalProvisionedAux : null,
            provision: 0,
            Subcategories: this.subCategoriesData
        }

        model.AnalyticId = this.AnalyticId
        model.ManagementReportId = this.managementReportId
        model.MonthYear = this.monthYear
        model.Employees = this.resources

        this.updateCostSubscrip = this.managementReportStaffService.PostCostDetailStaffMonth(this.managementReportId, model).subscribe(() => {
            this.costDetailMonthModal.hide();
            this.managementReport.updateBudgetView();
        },
            () => {
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
        this.resourcesSalarySubTotal = 0;
        this.resourcesChargesSubTotal = 0;
        var totalCharges = 0;
        var totalSalary= 0;

        this.resources.forEach(element => {
            this.totalCosts += element.total;
            this.resourcesSubTotal += element.total;
            this.resourcesSalarySubTotal += element.salary;
            this.resourcesChargesSubTotal += element.charges;
            totalCharges += element.charges;
            totalSalary += element.salary;
        });

        this.subCategoriesData.forEach(element => {
            this.totalCosts += element.value;
            this.categoriesSubTotal += element.value;
        });
        
        if(totalSalary > 0){
            this.totalChargesPercentage = (totalCharges/totalSalary)*100;
        }
    }

    getUsers() {
        this.getEmployeesSubscrip = this.employeeService.getListItems().subscribe(data => {
            this.users = data;
        },
        () => { });
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

    addSubcategoryData() {

        var cost = {
            costDetailStaffId: 0,
            id: this.subcategorySelected.id,
            name: this.subcategorySelected.name,
            nameCategory: this.categorySelected.name,
            monthYear: this.monthYear,
            description: "",
            value: 0,
            deleted: false
        }

        this.subCategoriesData.push(cost)
    }

    deleteSubcategory(index) {
        
        if (this.subCategoriesData[index].costDetailStaffId == 0) {
            this.subCategoriesData.splice(index, 1)
        }
        else {
            this.subCategoriesData[index].value = 0
            this.subCategoriesData[index].deleted = true
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
        var row1 = ["REAL", this.totalProvisioned, "TOTAL COSTOS", "", this.totalCosts];
        var row1Added = worksheet.addRow(row1);
        this.setTitleStyles(row1Added.getCell(1));
        this.setTitleStyles(row1Added.getCell(3));
        this.setCellNumber(row1Added.getCell(2));
        this.setCellNumber(row1Added.getCell(5));
        this.drawBorders(row1Added, 'bottom');
        this.drawCellBorder(row1Added.getCell(2), 'right');
        this.drawCellBorder(row1Added.getCell(4), 'right');
        worksheet.mergeCells(`C1:D1`);
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
        if (this.subCategoriesData && this.subCategoriesData.length > 0) {
            worksheet.addRow([]);
            var headerTable = ["Categoria", "SubCategoria", "Descripcion", "Monto"];
            var headerRow = worksheet.addRow(headerTable);
            headerRow.eachCell(cell => {
                this.setTitleStyles(cell);
            });
            this.drawBorders(headerRow, 'bottom');
            this.drawBorders(headerRow, 'top');
            this.subCategoriesData.forEach(expense => {
                var rowToAdd = [expense.nameCategory, expense.name, expense.description, expense.value];
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