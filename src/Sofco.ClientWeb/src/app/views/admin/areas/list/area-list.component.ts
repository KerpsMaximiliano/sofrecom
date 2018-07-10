import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "app/services/common/message.service";
import { Router } from "@angular/router";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { AreaService } from "../../../../services/admin/area.service";
declare var moment: any;

@Component({
    selector: 'app-area-list',
    templateUrl: './area-list.component.html'
})
  export class AreaListComponent implements OnInit, OnDestroy {

    public areas: any[] = new Array(); 

    public getSubscrip: Subscription;
    public deactivateSubscrip: Subscription;
    public activateSubscrip: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                public menuService: MenuService,
                private dataTableService: DataTableService,
                private areaService: AreaService,
                private errorHandlerService: ErrorHandlerService) { }

    ngOnInit(): void {
        this.getAll();
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.deactivateSubscrip) this.deactivateSubscrip.unsubscribe();
        if(this.activateSubscrip) this.activateSubscrip.unsubscribe();
    }

    getAll(){
        this.messageService.showLoading();

        this.getSubscrip = this.areaService.getAll().subscribe(response => {
            this.messageService.closeLoading();
            this.areas = response.data;
            this.initGrid();
        }, 
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);   
        });
    }

    goToDetail(area){
        this.router.navigate([`/admin/areas/${area.id}/edit`]);
    }

    habInhabClick(area){
        if (area.active){
            this.deactivate(area);
        } else {
            this.activate(area);
        }
    }

    deactivate(area){
        this.deactivateSubscrip = this.areaService.active(area.id, false).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                area.active = false;
                area.endDate = moment.now();
            },
            err => this.errorHandlerService.handleErrors(err));
    }
  
    activate(area){
        this.activateSubscrip = this.areaService.active(area.id, true).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                area.active = true;
                area.endDate = null;
                area.startDate = moment.now();
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    initGrid(){
        var columns = [0, 1, 2, 3];
        var title = `Areas-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#areaTable',
            columns: columns,
            title: title,
            withExport: true,
            columnDefs: [ {'aTargets': [2, 3], "sType": "date-uk"} ]
        }

        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
    }
}