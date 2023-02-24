import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { PurchaseOrderService } from "app/services/admin/purchase-order.service";


@Component({
    selector: 'purchase-orders-edit',
    templateUrl: './purchase-orders-edit.html'
})

export class PurchaseOrdersEditComponent implements OnInit{

    id: number;
    mode: string = "View";

    states = [
        { id: 0, name: "Todos" },
        { id: 1, name: "Pendiente Aprobación DAF" },
        { id: 2, name: "Pendiente Recepción Mercadería" },
        { id: 3, name: "Pendiente Recepción Factura" },
        { id: 4, name: "Cerrado" },
        { id: 5, name: "Rechazado" },
    ];
    estadoSeleccionado: number;
    purchaseOrder: any;
    showEdit: boolean = false;

    constructor(
        private activatedRoute: ActivatedRoute,
        private purchaseOrderService: PurchaseOrderService
    ) {}

    ngOnInit(): void {
        this.mode  = this.purchaseOrderService.getMode();
        this.id = this.activatedRoute.snapshot.params['id'];
        this.purchaseOrderService.getOCById(this.id).subscribe(d => {
            console.log(d.data);
            this.estadoSeleccionado = d.data.statusId;
            this.purchaseOrder = d.data;
            this.showEdit = true;
        });
    }
    
}