import { Component, Input, ViewChild, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { SolfacService } from 'app/services/billing/solfac.service';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { FileUploader } from 'ng2-file-upload';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { MessageService } from 'app/services/common/message.service';
import * as FileSaver from "file-saver";
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { MenuService } from 'app/services/admin/menu.service';
import { Option } from 'app/models/option';
import { CertificatesService } from 'app/services/billing/certificates.service';

declare var $: any;

@Component({
  selector: 'solfac-attachments',
  templateUrl: './solfac-attachments.component.html',
  styleUrls: ['./solfac-attachments.component.scss']
})
export class SolfacAttachmentsComponent implements OnInit, OnDestroy {

    @Input() solfacId: number;
    @Input() status: string;
    @ViewChild('selectedFile') selectedFile: any;
    @ViewChild('confirmDeleteFileModal') confirmModal;
    @ViewChild('pdfViewer') pdfViewer: any;
    
    public certificates: any[] = new Array();
    public certificatesRelated: any[] = new Array();
    public files: any[] = new Array<any>();
    fileId: number;
    index: number;
    certificateId: number;
    certificateName: string;
    
    getAttachmentsSubscrip: Subscription;
    getCertificateAvailableSubscrip: Subscription;
    getCertificatesRelatedSubscrip: Subscription;

    public uploader: FileUploader = new FileUploader({url:""});

    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmDeleteFileModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    constructor(private solfacService: SolfacService,
                private messageService: MessageService,
                private certificateService: CertificatesService,
                private menuService: MenuService,
                private errorHandlerService: ErrorHandlerService) {
    }

    ngOnInit() {
        this.getAttachments();
        this.getCertificatesRelated();

        this.uploader = new FileUploader({url: this.solfacService.getUrlForImportFile(this.solfacId), authToken: `Bearer ${Cookie.get('access_token')}`, maxFileSize: 10*1024*1024 });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            var json = JSON.parse(response);

            if(json.messages) this.messageService.showMessages(json.messages);

            var file = json.data;
            this.files.push({ id: file.id, name: file.name, creationDate: file.creationDate });
        };

        this.uploader.onSuccessItem = (item: any, response: any, status: any, headers: any) => {
            item.remove();
        };
    }

    ngOnDestroy(){
        if(this.getAttachmentsSubscrip) this.getAttachmentsSubscrip.unsubscribe();
        if(this.getCertificateAvailableSubscrip) this.getCertificateAvailableSubscrip.unsubscribe();
        if(this.getCertificatesRelatedSubscrip) this.getCertificatesRelatedSubscrip.unsubscribe();
    }

    getAttachments(){
        this.getAttachmentsSubscrip = this.solfacService.getAttachments(this.solfacId).subscribe(d => {
            this.files = d;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getCertificatesRelated(){
        this.getCertificatesRelatedSubscrip = this.solfacService.getCertificatesRelated(this.solfacId).subscribe(d => {
            this.certificatesRelated = d;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    clearSelectedFile(){
        this.uploader.clearQueue();
  
        this.selectedFile.nativeElement.value = '';
    }

    exportFile(file){
        this.solfacService.downloadFile(file.id).subscribe(response => {
            FileSaver.saveAs(response, file.name);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    showPdf(file){
        if(file.name.endsWith('.pdf')){
            this.pdfViewer.getAttachment(file.id);
        }
    }

    canUploadFiles(){
        return this.menuService.hasFunctionality("SOLFA", "AFILE");
    }

    deleteFile(){
        this.confirmModal.hide();
        this.messageService.showLoading();

        this.solfacService.deleteFile(this.fileId).subscribe(response => {
            if(response.messages) this.messageService.showMessages(response.messages);
            this.files.splice(this.index, 1);

            this.fileId = null;
            this.index = null;

            this.messageService.closeLoading();
        },
        err => {
            this.messageService.closeLoading();
            this.errorHandlerService.handleErrors(err)
        });
    }

    confirm() {}
 
    showConfirm(fileId, index){
        this.fileId = fileId;
        this.index = index;
        this.confirm = this.deleteFile;
        this.confirmModal.show();
    }

    getCertificatesAvailable(customerId){
        this.getCertificateAvailableSubscrip = this.certificateService.getByClient(customerId).subscribe(data => {
          this.certificates = data;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    exportCertificate(certificate){
        this.certificateService.exportFile(certificate.fileId).subscribe(file => {
            FileSaver.saveAs(file, certificate.fileName);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    deleteCertificate(){
        this.getCertificateAvailableSubscrip = this.solfacService.deleteCertificate(this.solfacId, this.certificateId).subscribe(response => {
            if(response.messages) this.messageService.showMessages(response.messages);
            this.certificatesRelated.splice(this.index, 1);

            this.certificates.push({ id: this.certificateId, text: this.certificateName });
            $("#certificate").select2('data', {id: this.certificateId.toString(), text: this.certificateName});   
          },
          err => this.errorHandlerService.handleErrors(err),
        () => this.confirmModal.hide());
    }

    showConfirmDeleteCertificate(certificate, index){
        this.certificateId = certificate.id;
        this.certificateName = certificate.name;
        this.index = index;
        this.confirm = this.deleteCertificate;
        this.confirmModal.show();
    }

    addCertificate(){
        var certificatesValues = <any>$('#certificate').val();
  
        if(certificatesValues && certificatesValues.length == 0) return;
  
        this.messageService.showLoading();

        this.solfacService.addCertificates(this.solfacId, certificatesValues).subscribe(data => {
          if(data.messages) this.messageService.showMessages(data.messages);
      
          if(data.data && data.data.length > 0){
  
            data.data.forEach(element => {
              this.certificatesRelated.push({ id: element.id, name: element.name, fileName: element.fileName, creationDate: element.creationDate, fileId: element.fileId });
            });
  
            this.certificates = this.certificates.filter(item => {
              if(!certificatesValues.includes(item.id.toString())) return item;
  
              return null;
            })
          }
        },
        err => this.errorHandlerService.handleErrors(err),
        () => this.messageService.closeLoading());
      }
}