import { Component } from "@angular/core";
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { PurchaseOrderService } from "app/services/admin/purchase-order.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { MessageService } from "app/services/common/message.service";
import { UserInfoService } from "app/services/common/user-info.service";

@Component({
    selector: 'purchase-orders-new',
    templateUrl: './purchase-orders-new.html'
})

export class PurchaseOrdersNew {

    notasPedido = [
        {id: 1, nombre: "Nota 1"},
        {id: 2, nombre: "Nota 2"},
        {id: 3, nombre: "Nota 3"},
        {id: 4, nombre: "Nota 4"},
    ];

    ordenCompraForm: FormGroup = new FormGroup({
        numberOC: new FormControl(null, [Validators.required, Validators.maxLength(10)]),
        proveedor: new FormControl(null, Validators.required),
        notape: new FormControl(null),
        montoOC: new FormControl(null, [Validators.required, Validators.max(999999999.99)])
    });
    formProductoServicio: FormGroup = new FormGroup({
        id: new FormControl(null),
        productService: new FormControl(null, [Validators.required]),
        quantity: new FormControl(null, [Validators.required, Validators.min(0)]),
        ammount: new FormControl(null, [Validators.required, Validators.min(0)])
    });
    formProductoServicioTable: FormGroup = new FormGroup({
        productoServicio: new FormArray([])
    });

    productsServicesTableError: boolean = true;
    requestNote: any;
    proveedores = [];
    productosServicios = [];
    finalProductosServicios = [];
    productsServicesError: boolean = false;
    productsServicesQuantityError: boolean = false;
    userInfo;
    

    constructor(
        private builder: FormBuilder,
        private requestNoteService: RequestNoteService,
        private purchaseOrderService: PurchaseOrderService,
        private messageService: MessageService,
        private router: Router
    ) {

    }

    ngOnInit(): void {
        this.userInfo = UserInfoService.getUserInfo();
        console.log(this.purchaseOrderService.getId());
        this.requestNoteService.getById(this.purchaseOrderService.getId()).subscribe(d => {
            this.requestNote = d.data;
            console.log(this.requestNote)
            this.ordenCompraForm.get('notape').setValue(this.requestNote.description);
            this.ordenCompraForm.get('notape').disable();
            this.proveedores = this.requestNote.providersSelected;
            this.productosServicios = this.requestNote.productsServices;
        })
    }

    agregarProductoServicio() {
        this.productsServicesTableError = false;
        if(this.formProductoServicio.invalid) {
            this.productsServicesTableError = true;
            this.markFormGroupTouched(this.formProductoServicio);
            return;
        };
        let productoServicio = {
            id: this.formProductoServicio.controls.productService.value,
            quantity: this.formProductoServicio.controls.quantity.value,
            ammount: this.formProductoServicio.controls.ammount.value
        };
        let find = this.productosServicios.find(p => p.id == this.formProductoServicio.controls.productService.value);
        if(find == undefined) {
            this.messageService.showMessage("Ya existe este producto o servicio en la grilla", 2);
            return;
        }
        this.getProductoServicio().push(this.builder.group({
            id: productoServicio.id,
            productService: find.productService,
            quantity: productoServicio.quantity,
            ammount: this.formProductoServicio.controls.ammount.value
        }));
        this.getProductoServicio().controls[this.getProductoServicio().controls.length - 1].get('productService').disable()
        this.finalProductosServicios.push(productoServicio);
        this.productsServicesError = false;
        this.formProductoServicio.get('productService').setValue(null);
        this.formProductoServicio.get('quantity').setValue(null);
        this.formProductoServicio.get('ammount').setValue(null);
    }

    eliminarProductoServicio(index: number) {
        this.finalProductosServicios.splice(index, 1);
        this.getProductoServicio().removeAt(index);
    }

    change(event: any) {
        console.log(event)
    }

    approve() {
        console.log(this.ordenCompraForm)
        this.getProductoServicio().controls.forEach(c => {
            console.log(c.get('productService').value)
        });
        if(this.ordenCompraForm.invalid) {
            this.markFormGroupTouched(this.ordenCompraForm);
            return;
        };
        if(this.finalProductosServicios.length == 0) {
            this.messageService.showMessage("Debe agregar al menos un producto/servicio/material", 2);
            return;
        };
        let finalProd = [];
        this.getProductoServicio().controls.forEach(c => {
            console.log(c.get('productService').value);
            finalProd.push({
                //id: null,
                description: c.get('productService').value,
                amount: c.get('ammount').value,
                quantity: c.get('quantity').value,
                requestNoteProductServiceId: c.get('id').value
            })
        });
        let model = {
            //id: null,
            requestNoteId: this.purchaseOrderService.getId(),
            userApplicantId: this.userInfo.id,
            workflowId: 9,
            providerId: this.ordenCompraForm.get('proveedor').value,
            providerDescription: this.proveedores.find(p => p.providerId == this.ordenCompraForm.get('proveedor').value).providerDescription,
            totalAmount: this.ordenCompraForm.get('montoOC').value,
            number: this.ordenCompraForm.get('numberOC').value,
            items: finalProd
            // statusId: ,
            // statusDescription: ,
            // hasEditPermissions: ,
            // hasReadPermissions: 
        };
        this.purchaseOrderService.saveOC(model).subscribe(d => {
            console.log(d);
            this.router.navigate(['/providers/purchase-orders']);
        })
    }

    getProductoServicio(): FormArray {
        return this.formProductoServicioTable.get("productoServicio") as FormArray;
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