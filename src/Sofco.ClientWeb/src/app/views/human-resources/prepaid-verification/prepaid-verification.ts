import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { UtilsService } from "app/services/common/utils.service";
import { I18nService } from "app/services/common/i18n.service";
import { MessageService } from "app/services/common/message.service";
import { PrepaidService } from "app/services/human-resources/prepaid.service";
import { DataTableService } from "app/services/common/datatable.service";
import { PrepaidImportedDataStatus } from "app/models/enums/prepaidImportedDataStatus";

@Component({
    selector: 'prepaid-verification',
    templateUrl: './prepaid-verification.html',
    styleUrls: ['./prepaid-verification.scss']
})
export class PrepaidVerificationComponent implements OnInit, OnDestroy {
    @ViewChild('selectedFile') selectedFile: any;

    getYearsSubscrip: Subscription;
    getMonthsSubscrip: Subscription;
    getDataSubscrip: Subscription;
    updateSubscrip: Subscription;
    closeSubscrip: Subscription;

    yearId: number;
    monthId: number;
    isClosed: boolean = false;
    gridVisible: boolean = false;

    years: any[] = new Array();
    months: any[] = new Array();
    data: any[] = new Array();
    allData: any[] = new Array();
    totals: any[] = new Array();
    provisioneds: any[] = new Array();
    resources: any[] = new Array();
    states: any[] = new Array();
    prepaids: any[] = new Array();

    prepaidSelected: string;
    stateSelected: string;
    resourceSelected: string;

    constructor(private utilsService: UtilsService,
                private i18nService: I18nService,
                private datatableService: DataTableService,
                private prepaidService: PrepaidService,
                private messageService: MessageService){
    }

    ngOnInit(): void {
        this.getYearsSubscrip = this.utilsService.getYears().subscribe(data => {
            this.years = data;
        });

        this.getMonthsSubscrip = this.utilsService.getMonths().subscribe(data => {
            this.months = data.map(item => {
                item.text = this.i18nService.translateByKey(item.text);
                return item;
            });
        });
    }

    ngOnDestroy(): void {
        if(this.getYearsSubscrip) this.getYearsSubscrip.unsubscribe();
        if(this.getMonthsSubscrip) this.getMonthsSubscrip.unsubscribe();
        if(this.getDataSubscrip) this.getDataSubscrip.unsubscribe();
        if(this.updateSubscrip) this.updateSubscrip.unsubscribe();
        if(this.closeSubscrip) this.closeSubscrip.unsubscribe();
    }

    search(){ 
        if(!this.yearId || !this.monthId || this.yearId <= 0 || this.monthId <= 0) {
            this.messageService.showErrorByFolder("rrhh/prepaid", "dateRequired");
            return;
        };

        this.messageService.showLoading();

        this.allData = [];
        this.data = [];
        this.prepaids = [];
        this.states = [];
        this.resources = [];

        this.gridVisible = false;

        this.getDataSubscrip = this.prepaidService.get(this.yearId, this.monthId).subscribe(response => {
            this.messageService.closeLoading();

            if(response && response.data && response.data.items.length > 0){
                this.gridVisible = true;

                this.allData = response.data.items.map(item => {
                    item.selected = false;

                    if(!this.prepaids.includes(item.prepaid.text)){
                        this.prepaids.push(item.prepaid.text);
                    }

                    if(!this.states.includes(item.status)){
                        this.states.push(item.status);
                    }

                    if(item && item != null && !this.resources.includes(`${item.employeeNumber} - ${item.employeeName}`)){
                        this.resources.push(`${item.employeeNumber} - ${item.employeeName}`);
                    }

                    return item;
                });

                this.data = this.allData;

                this.prepaids = this.prepaids.map(item => {
                    return { id: item, text: item };
                });

                this.states = this.states.map(item => {
                    return { id: item, text: this.getStatusDesc(item) };
                });

                this.resources = this.resources.map(item => {
                    return { id: item, text: item };
                });

                this.isClosed = response.data.isClosed;
                this.totals = response.data.totals;

                this.provisioneds = response.data.provisioneds.map(item => {
                    item.selected = false;
                    return item;
                });

                this.initGrid();
                this.initProvisionedGrid();
            }
            else{
                this.messageService.showWarningByFolder("common", "searchNotFound");
            }
        }, 
        error => this.messageService.closeLoading());
    }

    initGrid(){
        var columns = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 13];
        var title = "informacion-prepaga - " + this.monthId + "-" + this.yearId;

        var options = { 
            selector: "#searchTable",
            columns: columns, 
            title: title,
            withExport: true,
            columnDefs: [ {"aTargets": [5], "sType": "date-uk"} ],
        };

        this.datatableService.destroy(options.selector); 
        this.datatableService.initialize(options);

        setTimeout(() => {
            $("#searchTable_wrapper").css("float","left");
            $("#searchTable_wrapper").css("padding-bottom","50px");
            $("#searchTable_filter label").addClass('search-filter');
            $("#searchTable_paginate").addClass('table-pagination');
            $(".html5buttons").addClass('export-buttons');
            $("#searchTable_length").css("margin-right","10px");
            $("#searchTable_info").css("padding-top","4px");
        }, 500);
    }

    initProvisionedGrid(){
        var columns = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
        var title = "provisionados meses anteriores";

        var options = { 
            selector: "#provisionedTable",
            columns: columns, 
            title: title,
            withExport: true,
            columnDefs: [ {"aTargets": [9], "sType": "date-uk"} ],
        };

        this.datatableService.destroy(options.selector); 
        this.datatableService.initialize(options);

        setTimeout(() => {
            $("#provisionedTable_wrapper").css("float","left");
            $("#provisionedTable_wrapper").css("padding-bottom","50px");
            $("#provisionedTable_filter label").addClass('search-filter');
            $("#provisionedTable_paginate").addClass('table-pagination');
            $(".html5buttons").addClass('export-buttons');
            $("#provisionedTable_length").css("margin-right","10px");
            $("#provisionedTable_info").css("padding-top","4px");
        }, 500);
    }

    getStatusClass(status){
        switch(status){
            case PrepaidImportedDataStatus.Confirmed: return "label-primary";
            case PrepaidImportedDataStatus.Success: return "label-primary";
            case PrepaidImportedDataStatus.Provisioned: return "label-warning";
            case PrepaidImportedDataStatus.Error: return "label-danger";
            default: return '';
        }
    }

    getStatusDesc(status){
        switch(status){
            case PrepaidImportedDataStatus.Success: return "Sin diferencias";
            case PrepaidImportedDataStatus.Provisioned: return this.i18nService.translateByKey("provisioned");
            case PrepaidImportedDataStatus.Error: return this.i18nService.translateByKey("toVerify");
            case PrepaidImportedDataStatus.Confirmed: return this.i18nService.translateByKey("confirmed");
            default: return '';
        }
    }

    selectAllError(){
        this.data.forEach((item, index) => {
            if(item.status == PrepaidImportedDataStatus.Error){
                item.selected = true;
            }
        });
    }

    selectAllProvisioned(){
        this.data.forEach((item, index) => {
            if(item.status == PrepaidImportedDataStatus.Provisioned){
                item.selected = true;
            }
        });
    }

    selectAllSuccess(){
        this.data.forEach((item, index) => {
            if(item.status == PrepaidImportedDataStatus.Success){
                item.selected = true;
            }
        });
    }

    unselectAll(){
        this.data.forEach((item, index) => {
            if(item.status != PrepaidImportedDataStatus.Confirmed){
                item.selected = false;
            }
        });
    }

    noneItemsSelected(){
        return this.data.filter(x => x.selected == true).length == 0;
    }

    itemsHasNoErrors(){
        return this.data.filter(x => x.status == PrepaidImportedDataStatus.Error).length == 0;
    }

    allItemsAreConfirmed(){
        return this.data.every(x => x.status == PrepaidImportedDataStatus.Confirmed);
    }

    confirmAll(){
        var items = this.data.filter(x => x.selected == true).map(x => x.id);
        
        this.update({ ids: items, status: PrepaidImportedDataStatus.Confirmed });
    }

    toProvisionAll(){
        var items = this.data.filter(x => x.selected == true).map(x => x.id);
        
        this.update({ ids: items, status: PrepaidImportedDataStatus.Provisioned });
    }

    close(){
        this.messageService.showLoading();

        this.closeSubscrip = this.prepaidService.close(this.yearId, this.monthId).subscribe(data => {
            this.messageService.closeLoading();

            this.data.forEach((item, index) => {
                item.closed = true;
            });
        }, 
        error => this.messageService.closeLoading());
    }

    update(model){
        this.messageService.showLoading();

        this.updateSubscrip = this.prepaidService.update(model).subscribe(data => {
            this.messageService.closeLoading();

            this.data.forEach((item, index) => {
                if(item.selected == true){
                    item.status = model.status;
                    item.selected = false;
                }
            });

            this.initGrid();
        }, 
        error => this.messageService.closeLoading());
    }

    selectAllProvisionedTable(){
        this.provisioneds.forEach((item, index) => {
            item.selected = true;
        });
    }

    unselectAllProvisioned(){
        this.provisioneds.forEach((item, index) => {
            item.selected = false;
        });
    }

    noneItemsProvisionedSelected() {
        return this.provisioneds.filter(x => x.selected == true).length == 0;
    }

    confirmProvisioned(){
        var items = this.provisioneds.filter(x => x.selected == true).map(x => x.id);

        var model = { ids: items, status: PrepaidImportedDataStatus.Confirmed };

        this.messageService.showLoading();

        this.updateSubscrip = this.prepaidService.update(model).subscribe(data => {
            this.messageService.closeLoading();

            items.forEach((item, index) => {

                var itemToDelete = this.provisioneds.findIndex(x => x.id == item);

                if(itemToDelete >= 0){
                    this.provisioneds.splice(itemToDelete, 1);
                }
            });

            this.initProvisionedGrid();
        }, 
        error => this.messageService.closeLoading());
    }

    filter(){
        var itemsFiltered = [];

        this.allData.forEach((item, index) => {
            var addItem = true;

            if(this.stateSelected != undefined && item.status != this.stateSelected){
                addItem = false;
            }

            if(this.prepaidSelected && item.prepaid.text != this.prepaidSelected){
                addItem = false;
            }

            if(this.resourceSelected && `${item.employeeNumber} - ${item.employeeName}` != this.resourceSelected){
                addItem = false;
            }

            if(addItem) itemsFiltered.push(item);
        });

        this.data = itemsFiltered;
        this.initGrid();
    }

    clean(){
        this.stateSelected = null;
        this.prepaidSelected = null;
        this.resourceSelected = null;
    }
}