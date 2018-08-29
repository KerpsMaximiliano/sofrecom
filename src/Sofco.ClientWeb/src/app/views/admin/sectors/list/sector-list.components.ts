import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { MessageService } from "../../../../services/common/message.service";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { DataTableService } from "../../../../services/common/datatable.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { SectorService } from "../../../../services/admin/sector.service";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
declare var moment: any;

@Component({
    selector: 'app-sector-list',
    templateUrl: './sector-list.component.html'
})
  export class SectorListComponent implements OnInit, OnDestroy {

    public sectors: any[] = new Array(); 
    public sectorSelected: any;

    public getSubscrip: Subscription;
    public deactivateSubscrip: Subscription;
    public activateSubscrip: Subscription;

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(private messageService: MessageService,
                private router: Router,
                public menuService: MenuService,
                private dataTableService: DataTableService,
                private sectorService: SectorService) { }

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
        error => this.messageService.closeLoading());
    }

    goToDetail(sector){
        this.router.navigate([`/admin/sectors/${sector.id}/edit`]);
    }

    habInhabClick(sector){
        this.sectorSelected = sector

        if (sector.active){
            this.confirm = this.deactivate;    
        } else {
            this.confirm = this.activate; 
        }

        this.confirmModal.show();
    }

    confirm(){}

    deactivate(){
        this.deactivateSubscrip = this.sectorService.active(this.sectorSelected.id, false).subscribe(
            data => {
                this.sectorSelected.active = false;
                this.sectorSelected.endDate = moment.now();

                this.sectorSelected = null;
            },
            err => this.confirmModal.hide());
    }
  
    activate(){
        this.activateSubscrip = this.sectorService.active(this.sectorSelected.id, true).subscribe(
            data => {
                this.sectorSelected.active = true;
                this.sectorSelected.endDate = null;
                this.sectorSelected.startDate = moment.now();

                this.sectorSelected = null;
            },
            err => this.confirmModal.hide());
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