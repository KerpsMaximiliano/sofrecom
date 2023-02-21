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
    showEdit: boolean = false;

    data = [
        {
            id: 4,
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
            id: 10,
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
            id: 11,
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

    constructor(
        private activatedRoute: ActivatedRoute,
        private purchaseOrderService: PurchaseOrderService
    ) {}

    ngOnInit(): void {
        this.id = this.activatedRoute.snapshot.params['id'];
        if(this.id == 4) {
            this.estadoSeleccionado = 4;
        } else if (this.id == 10) {
            this.estadoSeleccionado = 10;
        } else if(this.id == 11){
            this.estadoSeleccionado = 11;
        } else {
            this.estadoSeleccionado = 12;
        }
        this.mode  = this.purchaseOrderService.getMode();
        
        this.showEdit = true;
    }
    
}