import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { Subscription } from "rxjs";
import { Invoice } from "../../../../models/billing/invoice/invoice";
import { InvoiceService } from "../../../../services/billing/invoice.service";
import { MessageService } from "../../../../services/common/message.service";
import * as FileSaver from "file-saver";
import { FileUploader } from 'ng2-file-upload';
import { MenuService } from "../../../../services/admin/menu.service";
import { InvoiceStatus } from "../../../../models/enums/invoiceStatus";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
import { CustomerService } from '../../../../services/billing/customer.service';
import { SolfacAccountControlComponent } from '../../solfac/solfac-account-control/solfac-account-control.component';
import { AuthService } from '../../../../services/common/auth.service';
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
    importMultile: boolean = false;

    @ViewChild('confirmModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @ViewChild('accountControl') accountControl: SolfacAccountControlComponent;

    constructor(private router: Router,
                private activatedRoute: ActivatedRoute,
                private service: InvoiceService,
                private authService: AuthService,
                public menuService: MenuService,
                private customerService: CustomerService,
                private messageService: MessageService) {}

    ngOnInit() {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.projectId = params['projectId'];
        });

        this.project = JSON.parse(sessionStorage.getItem("projectDetail"));
        this.customer = JSON.parse(sessionStorage.getItem("customer"));

        if(this.customer){
            this.model.customerId = this.customer.crmId;
            this.model.accountName = this.customer.name;
            this.model.address = this.customer.address;
            this.model.zipcode = this.customer.postalCode;
            this.model.city = this.customer.city;
            this.model.province = this.customer.province;
            this.model.country = this.customer.country;
            this.model.cuit = this.customer.cuit;
        }
        else{
            this.customerService.getById(sessionStorage.getItem("customerId")).subscribe(data => {
                this.model.customerId = data.crmId;
                this.model.accountName = data.name;
                this.model.address = data.address;
                this.model.zipcode = data.postalCode;
                this.model.city = data.city;
                this.model.province = data.province;
                this.model.country = data.country;
                this.model.cuit = data.cuit;

                sessionStorage.setItem("customer", JSON.stringify(data));
              });
        }

        this.model.project = this.project.name;
        this.model.projectId = this.projectId;
        this.model.analytic = this.project.analytic;
        this.model.invoiceStatus = InvoiceStatus[InvoiceStatus.SendPending];
        this.model.service = sessionStorage.getItem("serviceName");
        this.model.serviceId = sessionStorage.getItem("serviceId");
    }

    ngOnDestroy() {
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.sendToDafSubscrip) this.sendToDafSubscrip.unsubscribe();
    }

    save(){
        this.messageService.showLoading();

        this.service.add(this.model).subscribe(data => {
                this.messageService.closeLoading();

                this.configUploader(data.data.id);

                this.model.id = data.data.id;
            },
            err => this.messageService.closeLoading());
    }
 
    cancel(){
      this.router.navigate([`/billing/customers/${this.model.customerId}/services/${this.model.serviceId}/projects/${this.projectId}`]);
    }

    importMultipleChanged(){
        this.configUploader(this.model.id);
    }

    exportToExcel(){
        this.messageService.showLoading();

        this.service.exportNewExcelFile(this.model.id).subscribe(file => {
            this.messageService.closeLoading();
            FileSaver.saveAs(file, `REMITO_${this.model.accountName}_${this.model.service}_${this.model.project}_${this.getDateForFile()}.xlsx`);
        },
        () => this.messageService.closeLoading());
    }

    sendCallback(event){
        setTimeout(() => {
            this.cancel();
        }, 1500)
    }

    private configUploader(id){
        var url = this.importMultile ? this.service.getUrlForImportMultipleFile(id) : this.service.getUrlForImportFile(id);

        this.uploader = new FileUploader(
            {
                url: url, 
                authToken: 'Bearer ' + Cookie.get('access_token'), 
                maxFileSize: 10*1024*1024,
                allowedMimeType: ['application/vnd.ms-excel','application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'],
            }
        ); 

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            if(status == 401){
                this.authService.refreshToken().subscribe(token => {
                    this.messageService.closeLoading();

                    if(token){
                        this.clearSelectedFile();
                        this.messageService.showErrorByFolder('common', 'fileMustReupload');
                        this.configUploader(this.model.id);
                    }
                });

                return;
            }

            this.messageService.closeLoading();

            var dataJson = JSON.parse(response);
            
            if(dataJson.messages) this.messageService.showMessages(dataJson.messages);
            
            this.excelUploaded = true;
            this.model.excelFileName = dataJson.data.fileName;
            this.model.excelFileCreatedDate = new Date(dataJson.data.creationDate).toLocaleDateString();
            this.model.excelFileId = dataJson.data.id;
        };
    }

    delete(){
        this.service.delete(this.model.id).subscribe(data => {
            this.confirmModal.hide();

            setTimeout(() => { this.cancel(); }, 1500)
        });
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

    onAccountChange(data:any) {
        var accountDetail = this.accountControl.selected;
        this.model.cuit = accountDetail.cuit;
        this.model.address = accountDetail.address;
        this.model.zipcode = accountDetail.postalCode;
        this.model.city = accountDetail.city;
        this.model.province = accountDetail.province;
        this.model.country = accountDetail.country;
    }
}