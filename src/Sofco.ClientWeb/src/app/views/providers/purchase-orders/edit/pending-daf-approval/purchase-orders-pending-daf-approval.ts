import { Component, Input, OnInit, ViewChild } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { Router } from "@angular/router";
import { PurchaseOrderService } from "app/services/admin/purchase-order.service";
import { RequestNoteService } from "app/services/admin/request-note.service";

@Component({
    selector: 'purchase-orders-pending-daf-approval',
    templateUrl: './purchase-orders-pending-daf-approval.html'
})

export class PurchaseOrdersPendingDAFApproval implements OnInit {

    @ViewChild('workflow') workflow;
    private workflowModel: any;

    @Input() purchaseOrder: any;
    mode: string;

    proveedores = [
        {id: 1, nombre: "Uno"},
        {id: 2, nombre: "Dos"},
        {id: 3, nombre: "Tres"},
        {id: 4, nombre: "Cuatro"},
        {id: 5, nombre: "Cinco"},
    ];
    productosServicios = [];
    notasPedido = [
        {id: 1, nombre: "Nota 1"},
        {id: 2, nombre: "Nota 2"},
        {id: 3, nombre: "Nota 3"},
        {id: 4, nombre: "Nota 4"},
    ];

    ordenCompraForm: FormGroup = new FormGroup({
        numberOC: new FormControl(null, Validators.required),
        proveedor: new FormControl(2, Validators.required),
        numberNP: new FormControl(null, Validators.required),
        montoOC: new FormControl(12312, Validators.required)
    });

    requestNote: any;

    constructor(
        private requestNoteService: RequestNoteService,
        private purchaseOrderService: PurchaseOrderService,
        private router: Router
    ) {

    }

    ngOnInit(): void {
        console.log(this.purchaseOrder);
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
                this.workflow.init(this.workflowModel);
            }
            this.ordenCompraForm.get('numberNP').setValue(this.requestNote.description);
            this.ordenCompraForm.get('numberOC').setValue(this.purchaseOrder.number);
            this.ordenCompraForm.get('proveedor').setValue(this.purchaseOrder.providerDescription);
            this.ordenCompraForm.get('montoOC').setValue(this.purchaseOrder.totalAmount);

            this.productosServicios = this.purchaseOrder.items;
            
            this.purchaseOrderService.getAll({requestNoteId: this.purchaseOrder.requestNoteId}).subscribe(res => {
                console.log(res);
                //Filtrar por pendiente recepción factura (47) o finalizada (???)
            });
        });
        
    }

    change(event: any) {
        console.log(event)
    }

    approve() {

    }

    workflowClick() {
        this.workflow.updatePurchaseORder(this.purchaseOrder);
    }

    onTransitionSuccess() {
        this.router.navigate(["/providers/purchase-orders"]);
    }
    
}