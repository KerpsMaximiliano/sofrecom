import { Component, OnDestroy, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { MessageService } from "app/services/common/message.service";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { Subscription } from "rxjs/Subscription";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies/ng2-cookies";
import * as FileSaver from "file-saver";
import { CertificatesService } from "app/services/billing/certificates.service";

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

    public fileName: string;
    public creationDate: string;
    public fileId: number;

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
        this.form.model.clientExternalName = $('#clientExternalId option:selected').text();

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
                this.fileName = dataJson.data.fileName;
                this.creationDate = new Date(dataJson.data.creationDate).toLocaleDateString();
                this.fileId = dataJson.data.id;
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
        this.certificateService.exportFile(this.fileId).subscribe(file => {
            FileSaver.saveAs(file, this.fileName);
        },
        err => this.errorHandlerService.handleErrors(err));
    }
} 