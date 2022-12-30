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
        {id: 1, name: "Proveedor Uno"},
        {id: 2, name: "Proveedor Dos"},
        {id: 3, name: "Proveedor Tres"},
        {id: 4, name: "Proveedor Cuatro"},
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
    data = [
        {
            id: 1,
            name: "Orden de Compra 1",
            providerArea: "Uno",
            CUIT: 20111111114,
            ingresosBrutos: "Inscripto Convenio Multilateral",
            condicionIVA: "Monotributo",
            fecha: "2022-06-23",
            numeroOC: 2,
            numeroNP: 14,
            estado: "Pendiente Aprobación DAF"
        },
        {
            id: 2,
            name: "Orden de Compra 2",
            providerArea: "Dos",
            CUIT: 20222222224,
            ingresosBrutos: "Inscripto Convenio Multilateral",
            condicionIVA: "Monotributo",
            fecha: "2022-06-23",
            numeroOC: 2,
            numeroNP: 14,
            estado: "Pendiente Recepción Mercadería"
        },
        {
            id: 3,
            name: "Orden de Compra 3",
            providerArea: "Tres",
            CUIT: 20333333334,
            ingresosBrutos: "Inscripto Convenio Multilateral",
            condicionIVA: "Monotributo",
            fecha: "2022-06-23",
            numeroOC: 2,
            numeroNP: 14,
            estado: "Pendiente Recepción Factura"
        },

    ];
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
    }

    view(id) {
        this.purchaseOrderService.setMode("View");
        this.router.navigate([`providers/purchase-orders/edit/${id}`]);
    }

    edit(id) {
        this.purchaseOrderService.setMode("Edit");
        this.router.navigate([`providers/purchase-orders/edit/${id}`]);
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