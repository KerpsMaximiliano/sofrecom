import { Component, Input } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { PurchaseOrderService } from "app/services/admin/purchase-order.service";
import { RequestNoteService } from "app/services/admin/request-note.service";

@Component({
    selector: 'purchase-orders-pending-merchandise-reception',
    templateUrl: './purchase-order-pending-merchandise-reception.html'
})

export class PurchaseOrdersPendingMerchandiseReception {

    @Input() purchaseOrder: any;

    proveedores = [
        {id: 1, nombre: "Uno"},
        {id: 2, nombre: "Dos"},
        {id: 3, nombre: "Tres"},
        {id: 4, nombre: "Cuatro"},
        {id: 5, nombre: "Cinco"},
    ];
    productosServicios = [
        {id: 1, nombre: "Uno", monto: 10, cantidad: 10, pendientes: 10},
        {id: 2, nombre: "Dos", monto: 20, cantidad: 20, pendientes: 21},
        {id: 3, nombre: "Tres", monto: 34, cantidad: 45, pendientes: 55},
    ];
    notasPedido = [
        {id: 1, nombre: "Nota 1"},
        {id: 2, nombre: "Nota 2"},
        {id: 3, nombre: "Nota 3"},
        {id: 4, nombre: "Nota 4"},
    ];

    ordenCompraForm: FormGroup = new FormGroup({
        numberOC: new FormControl(2, Validators.required),
        proveedor: new FormControl(2, Validators.required),
        numberNP: new FormControl(null, Validators.required),
        montoOC: new FormControl(12312, Validators.required)
    });

    requestNote: any;

    constructor(
        private requestNoteService: RequestNoteService,
        private purchaseOrderService: PurchaseOrderService
    ) {

    }

    ngOnInit(): void {
        this.ordenCompraForm.disable();
        this.requestNoteService.getById(222).subscribe(d => {
            this.requestNote = d.data;
            console.log(this.requestNote);
            this.ordenCompraForm.get('numberNP').setValue(this.requestNote.description)
        })
    }

    change(event: any) {
        console.log(event)
    }

    approve() {

    }
}