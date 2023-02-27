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
import { environment } from "environments/environment";
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
    providers = [];
    states = [
        { id: null, name: "Todos" },
        { id: 4, name: "Pendiente Aprobación DAF" },
        { id: 34, name: "Pendiente Recepción Mercadería" },
        { id: 37, name: "Pendiente Recepción Factura" },
        //{ id: 4, name: "Cerrado" },
        //{ id: 5, name: "Rechazado" },
    ];
    providerId: number;
    numberOC: string = null;
    numberNP: number = null;
    dateSince: Date | string;
    dateTo: Date | string;
    stateId: number = null;

    businessName: string;
    areas = [];
    areaId: number;
    purchaseOrders = [];
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
        this.providersService.getAll().subscribe(d => {
            console.log(d);
            this.providers = d.data;
        })
        this.purchaseOrderService.getAll({}).subscribe(d => {
            console.log(d);
            this.purchaseOrders = d;
        });
        this.purchaseOrderService.getOCById(6).subscribe(d => {
            console.log(d);
        })
        // this.purchaseOrderService.getStates().subscribe(d => {
        //     console.log(d)
        // })
    }

    view(id) {
        this.purchaseOrderService.setMode("View");
        this.router.navigate([`providers/purchase-orders/edit/${id}`]);
    }

    edit(id) {
        this.purchaseOrderService.setMode("Edit");
        this.router.navigate([`providers/purchase-orders/edit/${id}`]);
    }

    extra(id) {
        this.purchaseOrderService.setMode("Edit");
        this.router.navigate([`providers/purchase-orders/edit/${id}`]);
    }

    refreshSearch() {
        this.providerId = null;
        this.numberOC = null;
        this.numberNP = null;
        this.dateSince = null;
        this.dateTo = null;
        this.stateId = null;
    }

    search() {
        if(this.dateTo != null && this.dateSince != null && this.dateTo <= this.dateSince) {
            this.messageService.showMessage("Fecha Hasta debe ser mayor a Fecha Desde", 2);
            return;
        };
        this.purchaseOrderService.getAll({
            requestNoteId: this.numberNP,
            fromDate: this.dateSince,
            toDate: this.dateTo,
            providerId: this.providerId,
            statusId: this.stateId,
            number: this.numberOC
        }).subscribe(d => {
            console.log(d);
            this.purchaseOrders = d;
        });
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

    get currentEnvironment() {
        return environment
    }
}