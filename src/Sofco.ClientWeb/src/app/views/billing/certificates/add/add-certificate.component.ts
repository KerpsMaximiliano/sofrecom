import { Component, OnDestroy, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies/ng2-cookies";
import * as FileSaver from "file-saver";
import { CertificatesService } from "app/services/billing/certificates.service";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

declare var $: any;

@Component({
    selector: 'add-certificate',
    templateUrl: './add-certificate.component.html',
    styleUrls: ['./add-certificate.component.scss']
})
export class NewCertificateComponent implements OnDestroy {

    @ViewChild('form') form;
    @ViewChild('selectedFile') selectedFile: any;

    addSubscrip: Subscription;

    public uploader: FileUploader = new FileUploader({url:""});
    public showUploader: boolean = false;

    public creationDate: string;

    @ViewChild('confirmDeleteFileModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmDeleteFileModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(private certificateService: CertificatesService,
                private router: Router,
                private messageService: MessageService,
                private errorHandlerService: ErrorHandlerService){
    }

    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
    }

    add() {
        this.messageService.showLoading();
        
        var client = this.form.customers.find(x => x.id == this.form.model.clientExternalId);

        if(client){
            this.form.model.clientExternalName = client.text;
        }

        this.addSubscrip = this.certificateService.add(this.form.model).subscribe(
            response => {
                this.messageService.closeLoading();
                if(response.messages) this.messageService.showMessages(response.messages);

                this.form.model.id = response.data.id
                this.uploaderConfig();
            },
            err => {
                this.messageService.closeLoading();
                this.errorHandlerService.handleErrors(err);
            });
    }

    uploaderConfig(){
        this.uploader = new FileUploader({url: this.certificateService.getUrlForImportFile(this.form.model.id),
                                          authToken: 'Bearer ' + Cookie.get('access_token') ,
                                          maxFileSize: 10*1024*1024
                                        });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            var dataJson = JSON.parse(response);
            
            if(dataJson.messages) this.messageService.showMessages(dataJson.messages);

            if(dataJson){
                this.creationDate = new Date(dataJson.data.creationDate).toLocaleDateString();

                this.form.model.fileId = dataJson.data.id;
                this.form.model.fileName = dataJson.data.fileName ;
            }

            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
        this.showUploader = true;
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    }

    exportExcel(){
        this.certificateService.exportFile(this.form.model.fileId).subscribe(file => {
            FileSaver.saveAs(file, this.form.model.fileName);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    deleteFile(){
        this.confirmModal.hide();
        this.messageService.showLoading();

        this.certificateService.deleteFile(this.form.model.id).subscribe(response => {
            if(response.messages) this.messageService.showMessages(response.messages);

            this.form.model.fileId = null;
            this.form.model.fileName = null;

            this.messageService.closeLoading();
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err)
        });
    }
} 