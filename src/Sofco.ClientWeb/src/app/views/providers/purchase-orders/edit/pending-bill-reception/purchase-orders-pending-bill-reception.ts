import { Component, Input, OnInit } from "@angular/core";
import { FormArray, FormControl, FormGroup, Validators } from "@angular/forms";

@Component({
    selector: 'purchase-orders-pending-bill-reception',
    templateUrl: './purchase-orders-pending-bill-reception.html'
})

export class PurchaseOrdersPendingBillReception implements OnInit {

    @Input() purchaseOrder: any;

    ocForm: FormGroup = new FormGroup({
        fecha: new FormControl(null, Validators.required),
        numeroFC: new FormControl(null, [Validators.required, Validators.pattern("([0-9]{4,5})(A|B|C)([0-9]{8})")]),
        impuesto: new FormControl(null, Validators.required)
    });
    detalleForm: FormGroup = new FormGroup({
        item: new FormControl(null, Validators.required),
        precio: new FormControl(null, [Validators.required, Validators.pattern("([0-9]{1,9}|0)([.][0-9]{0,2})?")]),
        cantidad: new FormControl(null, [Validators.required, Validators.pattern("([0-9]{1,10}|0)([.][0-9]{0,2})?")])
    });
    grillaForm: FormGroup = new FormGroup({
        items: new FormArray([])
    });
    facturaDate: any;
    showGrid: boolean = false;
    productosServicios = [];

    constructor() {}

    ngOnInit(): void {
        console.log(this.purchaseOrder);
        this.productosServicios = this.purchaseOrder.items;
    }

    dateChange() {
        this.ocForm.controls.fecha.setValue(this.facturaDate);
    }

    addItem() {
        if(this.detalleForm.invalid) {
            this.markFormGroupTouched(this.detalleForm);
            return;
        };
        this.showGrid = true;
        this.getItems().push(new FormGroup({
            itemGrilla: new FormControl(this.detalleForm.get('item').value, [Validators.required]),
            precioGrilla: new FormControl(this.detalleForm.get('precio').value, [Validators.required, Validators.pattern("([0-9]{1,9}|0)([.][0-9]{0,2})?")]),
            cantidadGrilla: new FormControl(this.detalleForm.get('cantidad').value, [Validators.required, Validators.pattern("([0-9]{1,10}|0)([.][0-9]{0,2})?")]),
        }));
        this.detalleForm.reset();
    }

    deleteItem(index: number) {
        this.getItems().removeAt(index);
        if(this.getItems().controls.length == 0) {
            this.showGrid = false;
        };
    }

    getItems(): FormArray {
        return this.grillaForm.get("items") as FormArray;
    }

    markFormGroupTouched(formGroup: FormGroup) {
        (<any>Object).values(formGroup.controls).forEach(control => {
            control.markAsTouched();

            if (control.controls) {
                this.markFormGroupTouched(control);
            }
        });
    };

}