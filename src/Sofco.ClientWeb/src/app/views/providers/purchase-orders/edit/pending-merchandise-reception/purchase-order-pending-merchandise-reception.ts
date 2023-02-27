import { Component, Input, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { PurchaseOrderService } from "app/services/admin/purchase-order.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'purchase-orders-pending-merchandise-reception',
    templateUrl: './purchase-order-pending-merchandise-reception.html'
})

export class PurchaseOrdersPendingMerchandiseReception {

    @ViewChild('workflow') workflow;
    private workflowModel: any;

    @Input() purchaseOrder: any;
    mode: string;

    productosServicios = [];

    ordenCompraForm: FormGroup = new FormGroup({
        numberOC: new FormControl(null, Validators.required),
        proveedor: new FormControl(null, Validators.required),
        numberNP: new FormControl(null, Validators.required),
        montoOC: new FormControl(null, Validators.required)
    });
    productosServiciosForm: FormGroup = new FormGroup({});

    requestNote: any;

    constructor(
        private requestNoteService: RequestNoteService,
        private purchaseOrderService: PurchaseOrderService,
        private router: Router,
        private messageService: MessageService
    ) {

    }

    ngOnInit(): void {
        this.ordenCompraForm.disable();
        this.mode = this.purchaseOrderService.getMode();
        this.requestNoteService.getById(this.purchaseOrder.requestNoteId).subscribe(d => {
            this.requestNote = d.data;
            console.log(this.requestNote);
            if(this.mode == "Edit") {
                this.workflowModel = {
                    workflowId: this.purchaseOrder.workflowId,
                    entityController: "buyOrderRequestNote",
                    entityId: this.purchaseOrder.id,
                    actualStateId: this.purchaseOrder.statusId
                };
                this.workflow.setCustomValidations(true);
                this.workflow.init(this.workflowModel);
            }
            this.ordenCompraForm.get('numberNP').setValue(this.requestNote.description);
            this.ordenCompraForm.get('numberOC').setValue(this.purchaseOrder.number);
            this.ordenCompraForm.get('proveedor').setValue(this.purchaseOrder.providerDescription);
            this.ordenCompraForm.get('montoOC').setValue(this.purchaseOrder.totalAmount);
            this.productosServicios = this.purchaseOrder.items;
            this.productosServicios.forEach(ps => {
                this.productosServiciosForm.addControl(`control${ps.id}`, new FormControl(null, Validators.required));
            })
        });
    }

    change(event: any) {
        console.log(event)
    }

    approve() {

    }

    workflowClick() {
        if(this.productosServiciosForm.invalid) {
            this.messageService.showMessage("Cada Producto/Servicio/Material debe tener una cantidad recepcionada", 2);
            this.workflow.setCustomValidations(true);
            return;
        };
        this.purchaseOrder.items.forEach(ps => {
            ps.deliveredQuantity = this.productosServiciosForm.get(`control${ps.id}`).value
        });
        console.log(this.purchaseOrder);
        this.workflow.updatePurchaseORder(this.purchaseOrder);
        this.workflow.setCustomValidations(false);
    }

    onTransitionSuccess() {
        this.router.navigate(["/providers/purchase-orders"]);
    }
}