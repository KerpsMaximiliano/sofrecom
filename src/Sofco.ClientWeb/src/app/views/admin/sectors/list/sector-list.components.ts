import { Component, OnInit, OnDestroy } from "@angular/core";
import { MessageService } from "app/services/common/message.service";
import { Router } from "@angular/router";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from "app/services/admin/menu.service";
import { SectorService } from "../../../../services/admin/sector.service";
declare var moment: any;

@Component({
    selector: 'app-sector-list',
    templateUrl: './sector-list.component.html'
})
  export class SectorListComponent implements OnInit, OnDestroy {

    public sectors: any[] = new Array(); 

    public getSubscrip: Subscription;
    public deactivateSubscrip: Subscription;
    public activateSubscrip: Subscription;

    constructor(private messageService: MessageService,
                private router: Router,
                public menuService: MenuService,
                private dataTableService: DataTableService,
                private sectorService: SectorService,
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

        this.getSubscrip = this.sectorService.getAll().subscribe(response => {
            this.messageService.closeLoading();
            this.sectors = response.data;
            this.initGrid();
        }, 
        error => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(error);   
        });
    }

    goToDetail(sector){
        this.router.navigate([`/admin/sectors/${sector.id}/edit`]);
    }

    habInhabClick(sector){
        if (sector.active){
            this.deactivate(sector);
        } else {
            this.activate(sector);
        }
    }

    deactivate(sector){
        this.deactivateSubscrip = this.sectorService.active(sector.id, false).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                sector.active = false;
                sector.endDate = moment.now();
            },
            err => this.errorHandlerService.handleErrors(err));
    }
  
    activate(sector){
        this.activateSubscrip = this.sectorService.active(sector.id, true).subscribe(
            data => {
                if(data.messages) this.messageService.showMessages(data.messages);
                sector.active = true;
                sector.endDate = null;
                sector.startDate = moment.now();
            },
            err => this.errorHandlerService.handleErrors(err));
    }

    initGrid(){
        var columns = [0, 1, 2, 3];
        var title = `Sectores-${moment(new Date()).format("YYYYMMDD")}`;

        var params = {
            selector: '#sectorTable',
            columns: columns,
            title: title,
            withExport: true,
            columnDefs: [ {'aTargets': [2, 3], "sType": "date-uk"} ]
        }

        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
    }
}