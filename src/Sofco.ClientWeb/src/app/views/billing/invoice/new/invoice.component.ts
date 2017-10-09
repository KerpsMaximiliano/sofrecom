import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { Invoice } from "app/models/billing/invoice/invoice";
import { InvoiceService } from "app/services/billing/invoice.service";
import { MessageService } from "app/services/common/message.service";
import * as FileSaver from "file-saver";
import { FileUploader } from 'ng2-file-upload';
import { MenuService } from "app/services/admin/menu.service";
import { InvoiceStatus } from "app/models/enums/invoiceStatus";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { I18nService } from 'app/services/common/i18n.service';
declare var $: any;

@Component({
  selector: 'app-invoice',
  templateUrl: './invoice.component.html',
  styleUrls: ['./invoice.component.scss']
})
export class InvoiceComponent implements OnInit, OnDestroy {

    @ViewChild('selectedFile') selectedFile: any;
    
    public model: Invoice = new Invoice();
    paramsSubscrip: Subscription;
    sendToDafSubscrip: Subscription;
    projectId: string;
    project: any;
    customer: any;
    public uploader: FileUploader;
    excelUploaded: boolean = false;

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(private router: Router,
                private activatedRoute: ActivatedRoute,
                private service: InvoiceService,
                public menuService: MenuService,
                private i18nService: I18nService,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService) {}

    ngOnInit() {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.projectId = params['projectId'];
        });

        this.project = JSON.parse(sessionStorage.getItem("projectDetail"));
        this.customer = JSON.parse(sessionStorage.getItem("customer"));

        this.model.accountName = this.customer.nombre;
        this.model.address = this.customer.address;
        this.model.zipcode = this.customer.postalCode;
        this.model.city = this.customer.city;
        this.model.province = this.customer.province;
        this.model.country = this.customer.country;
        this.model.cuit = this.customer.cuit;
        this.model.project = this.project.nombre;
        this.model.projectId = this.projectId;
        this.model.analytic = this.project.analytic;
        this.model.invoiceStatus = InvoiceStatus[InvoiceStatus.SendPending];
        this.model.service = sessionStorage.getItem("serviceName");
        this.model.serviceId = sessionStorage.getItem("serviceId");
        this.model.customerId = this.customer.id;
    }

    ngOnDestroy() {
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.sendToDafSubscrip) this.sendToDafSubscrip.unsubscribe();
    }

    save(){
      this.service.add(this.model).subscribe(
        data => {
          if(data.messages) this.messageService.showMessages(data.messages);

          this.configUploader(data.data.id);

          this.model.id = data.data.id;

          this.exportToExcel();
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    cancel(){
      this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects/${this.projectId}`]);
    }

    exportToExcel(){
        this.service.export(this.model).subscribe(file => {
            FileSaver.saveAs(file, `REMITO_${this.model.accountName}_${this.model.service}_${this.model.project}_${this.getDateForFile()}.xlsx`);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    sendCallback(event){
        setTimeout(() => {
            this.cancel();
        }, 1500)
    }

    private configUploader(id){
        this.uploader = new FileUploader(
            {
                url: this.service.getUrlForImportExcel(id), 
                authToken: 'Bearer ' + Cookie.get('access_token'), 
                maxFileSize: 10*1024*1024
            }
        );
        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            this.excelUploaded = true;
            this.messageService.succes(this.i18nService.translate("billing.invoice.excelAddedSucces"));
            this.model.excelFileName = "uploaded";
        };
    }

    delete(){
        this.service.delete(this.model.id).subscribe(data => {
            this.confirmModal.hide();

            if(data.messages) this.messageService.showMessages(data.messages);

            setTimeout(() => { this.cancel(); }, 1500)
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    canDelete(){
        if(this.model.id > 0 && this.model.invoiceStatus == InvoiceStatus[InvoiceStatus.SendPending] && this.menuService.hasFunctionality("REM", "RMV")){
            return true;
        }

        return false;
    }

    private getDateForFile(){
        var date = new Date();

        var yyyy = date.getFullYear().toString();
        var mm = (date.getMonth()+1).toString();
        var dd  = date.getDate().toString();

        var mmChars = mm.split('');
        var ddChars = dd.split('');

        return yyyy + (mmChars[1]?mm:"0"+mmChars[0]) + (ddChars[1]?dd:"0"+ddChars[0]);
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    }

    canUploadExcel(){
        if(this.model.id > 0 && !this.excelUploaded && this.menuService.hasFunctionality("REM", "ADEXC")){
            return true;
        }

        return false;
    }
}