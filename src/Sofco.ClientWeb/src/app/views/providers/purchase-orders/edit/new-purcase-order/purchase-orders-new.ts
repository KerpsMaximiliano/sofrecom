import { Component } from "@angular/core";
import { FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";

@Component({
    selector: 'purchase-orders-new',
    templateUrl: './purchase-orders-new.html'
})

export class PurchaseOrdersNew {

    proveedores = [
        {id: 1, nombre: "Uno"},
        {id: 2, nombre: "Dos"},
        {id: 3, nombre: "Tres"},
        {id: 4, nombre: "Cuatro"},
        {id: 5, nombre: "Cinco"},
    ];
    productosServicios = [
        {id: 1, nombre: "Uno"},
        {id: 2, nombre: "Dos"},
        {id: 3, nombre: "Tres"},
    ];
    notasPedido = [
        {id: 1, nombre: "Nota 1"},
        {id: 2, nombre: "Nota 2"},
        {id: 3, nombre: "Nota 3"},
        {id: 4, nombre: "Nota 4"},
    ];

    ordenCompraForm: FormGroup = new FormGroup({
        numberOC: new FormControl(null, Validators.required),
        proveedor: new FormControl(null, Validators.required),
        numberNP: new FormControl(null, Validators.required),
        montoOC: new FormControl(null, Validators.required)
    });

    constructor(
        private builder: FormBuilder
    ) {

    }

    ngOnInit(): void {

    }

    change(event: any) {
        console.log(event)
    }

    approve() {

    }
}