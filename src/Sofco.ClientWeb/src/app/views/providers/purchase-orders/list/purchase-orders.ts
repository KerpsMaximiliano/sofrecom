import { Component, OnInit } from "@angular/core";
import { NULL_INJECTOR } from "@angular/core/src/render3/component";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { IVAConditions } from "app/models/enums/IVAConditions";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { PurchaseOrderService } from "app/services/admin/purchase-order.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { forkJoin } from "rxjs";

@Component({
    selector: 'purchase-orders',
    templateUrl: './purchase-orders.html'
})

export class PurchaseOrdersComponent implements OnInit{


    ivaConditions = [
        'Resp. Inscripto',
        'Resp. No Inscripto',
        'Monotributo',
        'Exento/No Resp.'
    ];
    grossIncome = [
        'Inscripto en régimen local General',
        'Inscripto en régimen local simplificado',
        'Inscripto Convenio Multilateral',
        'Exento',
        'No aplica'
    ];
    providers = [
        {id: 1, name: "Uno"},
        {id: 2, name: "Dos"},
        {id: 3, name: "Tres"},
        {id: 4, name: "Cuatro"},
    ];
    states = [
        { id: 0, name: "Todos" },
        { id: 1, name: "Pendiente Aprobación DAF" },
        { id: 2, name: "Pendiente Recepción Mercadería" },
        { id: 3, name: "Pendiente Recepción Factura" },
        { id: 4, name: "Cerrado" },
        { id: 5, name: "Rechazado" },
    ];
    providerId: number;
    numberOC: number = null;
    numberNP: number = null;
    dateSince: Date;
    dateTo: Date;
    stateId: number = 0;

    businessName: string;
    areas = [];
    areaId: number;
    data = [];
    dataBackup = [];

    constructor(
        private router: Router,
        private providersService: ProvidersService,
        private providersArea: ProvidersAreaService,
        private purchaseOrderService: PurchaseOrderService,
        private dataTableService: DataTableService,
        private messageService: MessageService
    ) {}

    ngOnInit(): void {
        this.providersArea.getAll().subscribe(d => {
            d.data.forEach(providerArea => {
                if(providerArea.active) {
                    this.areas.push(providerArea);
                    this.areas = [...this.areas]
                }
            })
        });
        forkJoin([
            this.providersService.getAll(),
            this.providersArea.getAll()
        ]).subscribe(res => {
            res[0].data.forEach(provider => {
                let area = res[1].data.find(provArea => provArea.id == provider.providerAreaId);
                let model = {
                    id: provider.id,
                    name: provider.name,
                    providerArea: null,
                    providerAreaId: null,
                    CUIT: provider.cuit,
                    ingresosBrutos: this.grossIncome[provider.ingresosBrutos + 1],
                    condicionIVA: this.ivaConditions[provider.condicionIVA + 1],
                    active: provider.active
                }
                if(area != undefined) {
                    model.providerArea = area.description,
                    model.providerAreaId = area.id
                }
                this.data.push(model);
                this.data = [...this.data]
            });
            this.dataBackup = this.data;
            this.initGrid();
        });
    }

    view(id) {
        this.purchaseOrderService.setMode("View");
        this.router.navigate([`providers/purchase-orders/edit/${id}`]);
    }

    edit(id) {
        this.purchaseOrderService.setMode("Edit");
        this.router.navigate([`providers/purchse-orders/edit/${id}`])
    }

    refreshSearch() {
        this.providerId = null;
        this.numberOC = null;
        this.numberNP = null;
        this.dateSince = null;
        this.dateTo = null;
        this.stateId = 0;
    }

    search() {
        if(this.dateTo <= this.dateSince) {
            this.messageService.showMessage("Fecha Hasta debe ser mayor a Fecha Desde", 2);
        }
    }

    initGrid() {
        var columns = [0, 1, 2, 3, 4, 5, 6];
    
        var params = {
            selector: '#dataTable',
            columns: columns,
            title: 'Proveedores',
            withExport: true
        }
    
        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
    }

    collapse() {
        if ($("#collapseOne").hasClass('in')) {
            $("#collapseOne").removeClass('in');
        }
        else {
            $("#collapseOne").addClass('in');
        }

        this.changeIcon();
    }

    changeIcon() {
        if ($("#collapseOne").hasClass('in')) {
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
        else {
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        }
    }
}