import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { FormArray, FormControl, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { PurchaseOrderService } from "app/services/admin/purchase-order.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { AuthService } from "app/services/common/auth.service";
import { MessageService } from "app/services/common/message.service";
import { Cookie } from "ng2-cookies";
import { FileUploader } from "ng2-file-upload";

@Component({
    selector: 'purchase-orders-finalised',
    templateUrl: './purchase-orders-finalised.html'
})

export class PurchaseOrdersFinalised implements OnInit {

    @Input() purchaseOrder: any;

    mode: string;

    ocForm: FormGroup = new FormGroup({
        fecha: new FormControl(null, Validators.required),
        numeroFC: new FormControl(null, [Validators.required, Validators.pattern("([0-9]{4,5})(A|B|C)([0-9]{8})")]),
        impuesto: new FormControl(null, [Validators.required, Validators.maxLength(1)])
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
    productosServiciosAnteriores = [];
    requestNote: any;

    uploadedFile: any;

    ordenCompraForm: FormGroup = new FormGroup({
        numberOC: new FormControl(null, Validators.required),
        proveedor: new FormControl(null, Validators.required),
        numberNP: new FormControl(null, Validators.required),
        montoOC: new FormControl(null, Validators.required)
    });

    itemsFactura = [];

    constructor(
        private authService: AuthService,
        private messageService: MessageService,
        private requestNoteService: RequestNoteService,
        private purchaseOrderService: PurchaseOrderService,
        private router: Router
    ) {}

    ngOnInit(): void {
        console.log(this.purchaseOrder);
        this.productosServicios = this.purchaseOrder.items;
        this.mode = this.purchaseOrderService.getMode();
        this.requestNoteService.getById(this.purchaseOrder.requestNoteId).subscribe(d => {
            this.requestNote = d.data;
            console.log(this.requestNote);
            this.ordenCompraForm.get('numberNP').setValue(this.requestNote.description);
            this.ordenCompraForm.get('numberOC').setValue(this.purchaseOrder.number);
            this.ordenCompraForm.get('proveedor').setValue(this.purchaseOrder.providerDescription);
            this.ordenCompraForm.get('montoOC').setValue(this.purchaseOrder.totalAmount);
            this.ordenCompraForm.disable();
            this.ocForm.get('fecha').setValue(this.purchaseOrder.invoice.date);
            this.ocForm.get('numeroFC').setValue(this.purchaseOrder.invoice.number);
            this.ocForm.get('impuesto').setValue(this.purchaseOrder.invoice.taxCode);
            this.ocForm.disable();
            this.purchaseOrder.invoice.items.forEach(i => {
                let find = this.purchaseOrder.items.find(ps => ps.id == i.buyOrderProductServiceId);
                this.itemsFactura.push({
                    productService: find.description,
                    amount: i.amount,
                    quantity: i.quantity
                })
            });
            this.showGrid = true;

            this.productosServiciosAnteriores = this.purchaseOrder.items;
            this.productosServicios = this.purchaseOrder.items;
        });
    }

    dateChange() {
        this.ocForm.controls.fecha.setValue(this.facturaDate);
    }

    addItem() {
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

    downloadFile() {
        this.requestNoteService.downloadFile(this.purchaseOrder.invoice.fileId, 6, this.purchaseOrder.invoice.fileDescription);
    }

}