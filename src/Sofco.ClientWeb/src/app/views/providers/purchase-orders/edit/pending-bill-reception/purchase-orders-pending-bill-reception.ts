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
    selector: 'purchase-orders-pending-bill-reception',
    templateUrl: './purchase-orders-pending-bill-reception.html'
})

export class PurchaseOrdersPendingBillReception implements OnInit {

    @Input() purchaseOrder: any;

    @ViewChild('workflow') workflow;
    private workflowModel: any;
    mode: string;

    @ViewChild('selectedFile') selectedFile: any;
    public uploader: FileUploader = new FileUploader({
        url: this.purchaseOrderService.getUrlForImportFile(),
        authToken: 'Bearer ' + Cookie.get('access_token'), 
    });

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
            this.ordenCompraForm.disable();
            this.productosServiciosAnteriores = this.purchaseOrder.items;
            this.productosServicios = this.purchaseOrder.items;
        });
        this.uploaderConfig();
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

    uploaderConfig(){
        this.uploader = new FileUploader({url: this.purchaseOrderService.getUrlForImportFile(),
            authToken: 'Bearer ' + Cookie.get('access_token') ,
        });
        
        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            console.log(status);
            console.log(item);
            if(status == 401){
                this.authService.refreshToken().subscribe(token => {
                    this.messageService.closeLoading();

                    if(token){
                        this.clearSelectedFile();
                        this.messageService.showErrorByFolder('common', 'fileMustReupload');
                        this.uploaderConfig();
                    }
                });
                return;
            };
            let jsonResponse = JSON.parse(response);
            this.uploadedFile = {
                id: jsonResponse.data[0].id,
                name: jsonResponse.data[0].fileName
            };
            console.log(this.uploadedFile)
            this.clearSelectedFile();
        };
        this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    }

    workflowClick() {
        console.log(this.getItems().controls);
        let grillaCheck = false;
        if(this.ocForm.invalid) {
            this.markFormGroupTouched(this.ocForm)
            this.workflow.setCustomValidations(true);
            return;
        };
        if(this.facturaDate == null) {
            this.messageService.showMessage("Debe seleccionar una fecha", 2);
            this.workflow.setCustomValidations(true);
            return;
        };
        if(this.getItems().controls.length == 0) {
            this.messageService.showMessage("Debe agregar al menos un producto/servicio/material a la lista", 2);
            this.workflow.setCustomValidations(true);
            return;
        };
        this.getItems().controls.forEach(ctrl => {
            if(ctrl.invalid) {
                grillaCheck = true;
            }
        });
        if(grillaCheck) {
            this.messageService.showMessage("Los campos de la grilla deben estar completos", 2);
            this.workflow.setCustomValidations(true);
            return;
        };
        let invoice = {
            //id: ,
            buyOrderId: this.purchaseOrder.id,
            date: this.facturaDate,
            number: this.ocForm.get('numeroFC').value,
            taxCode: this.ocForm.get('impuesto').value,
            fileId: this.uploadedFile.id,
            fileDescription: this.uploadedFile.name,
            items: []
        };
        this.getItems().controls.forEach(ctrl => {
            invoice.items.push({
                amount: ctrl.get('precioGrilla').value,
                quantity: ctrl.get('cantidadGrilla').value,
                buyOrderProductServiceId: ctrl.get('itemGrilla').value,
            });
        });
        Object.assign(this.purchaseOrder, {invoice: invoice});
        console.log(this.purchaseOrder);
        this.workflow.setCustomValidations(false);
        this.workflow.updatePurchaseORder(this.purchaseOrder);
    }

    onTransitionSuccess() {
        this.router.navigate(["/providers/purchase-orders"]);
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