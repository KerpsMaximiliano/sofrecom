import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { MessageService } from "../../../../services/common/message.service";
import { Router } from "@angular/router";
import { Subscription } from "rxjs";
import { DataTableService } from "../../../../services/common/datatable.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { AreaService } from "../../../../services/admin/area.service";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
declare var moment: any;

@Component({
    selector: 'app-area-list',
    templateUrl: './area-list.component.html'
})
  export class AreaListComponent implements OnInit, OnDestroy {

    public areas: any[] = new Array(); 
    private areaSelected: any;
 
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
                private areaService: AreaService) { }

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
        error => this.messageService.closeLoading());
    }

    goToDetail(area){
        this.router.navigate([`/admin/areas/${area.id}/edit`]);
    }

    habInhabClick(area){
        this.areaSelected = area

        if (area.active){
            this.confirm = this.deactivate;    
        } else {
            this.confirm = this.activate; 
        }

        this.confirmModal.show();
    }

    confirm(){}

    deactivate(){
        this.deactivateSubscrip = this.areaService.active(this.areaSelected.id, false).subscribe(
            data => {
                this.areaSelected.active = false;
                this.areaSelected.endDate = moment.now();

                this.areaSelected = null;
            },
            err => this.confirmModal.hide());
    }
  
    activate(){
        this.activateSubscrip = this.areaService.active(this.areaSelected.id, true).subscribe(
            data => {
                this.areaSelected.active = true;
                this.areaSelected.endDate = null;
                this.areaSelected.startDate = moment.now();

                this.areaSelected = null;
            },
            err => this.confirmModal.hide());
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