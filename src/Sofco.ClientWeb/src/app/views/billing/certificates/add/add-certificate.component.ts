import { Component, OnDestroy, ViewChild } from "@angular/core";
import { MessageService } from "../../../../services/common/message.service";
import { Subscription } from "rxjs";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies/ng2-cookies";
import * as FileSaver from "file-saver";
import { CertificatesService } from "../../../../services/billing/certificates.service";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
import { AuthService } from "../../../../services/common/auth.service";

@Component({
    selector: 'add-certificate',
    templateUrl: './add-certificate.component.html',
    styleUrls: ['./add-certificate.component.scss']
})
export class NewCertificateComponent implements OnDestroy {

    @ViewChild('form') form;
    @ViewChild('selectedFile') selectedFile: any;
    @ViewChild('pdfViewer') pdfViewer;

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
                private authService: AuthService,
                private messageService: MessageService){
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

                this.form.model.id = response.data.id
                this.uploaderConfig();
            },
            () => this.messageService.closeLoading());
    }

    uploaderConfig(){
        this.uploader = new FileUploader({url: this.certificateService.getUrlForImportFile(this.form.model.id),
                                          authToken: 'Bearer ' + Cookie.get('access_token') ,
                                          maxFileSize: 10*1024*1024
                                        });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
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
            }

            this.messageService.closeLoading();

            var dataJson = JSON.parse(response);
            
            if(dataJson){
                if(dataJson.messages) this.messageService.showMessages(dataJson.messages);
                
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
        });
    }

    viewFile(){
        if(this.form.model.fileName.endsWith('.pdf')){
            this.certificateService.getFile(this.form.model.fileId).subscribe(response => {
                this.pdfViewer.renderFile(response.data);
            });
        }
    }

    deleteFile(){
        this.confirmModal.hide();
        this.messageService.showLoading();

        this.certificateService.deleteFile(this.form.model.id).subscribe(() => {
            this.form.model.fileId = null;
            this.form.model.fileName = null;
            this.messageService.closeLoading();
        },
        () => this.messageService.closeLoading());
    }
} 