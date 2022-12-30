import { Component, OnInit } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";

@Component({
    selector: 'purchase-orders-pending-daf-approval',
    templateUrl: './purchase-orders-pending-daf-approval.html'
})

export class PurchaseOrdersPendingDAFApproval implements OnInit {

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
        numberNP: new FormControl(2, Validators.required),
        montoOC: new FormControl(12312, Validators.required)
    });

    constructor(
        private builder: FormBuilder
    ) {

    }

    ngOnInit(): void {
        this.ordenCompraForm.disable();
    }

    change(event: any) {
        console.log(event)
    }

    approve() {

    }
    
}