import { Component, Input, ViewChild, OnDestroy, OnInit, Output, EventEmitter } from '@angular/core';
import { Subscription } from 'rxjs';
import { SolfacService } from '../../../../services/billing/solfac.service';
import { FileUploader } from 'ng2-file-upload';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { MessageService } from '../../../../services/common/message.service';
import * as FileSaver from "file-saver";
import { Ng2ModalConfig } from '../../../../components/modal/ng2modal-config';
import { MenuService } from '../../../../services/admin/menu.service';
import { CertificatesService } from '../../../../services/billing/certificates.service';

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

    @Output() onViewPdf: EventEmitter<any> = new EventEmitter();

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
                private menuService: MenuService) {
    }

    ngOnInit() {
        this.getAttachments();
        this.getCertificatesRelated();

        this.uploader = new FileUploader({url: this.solfacService.getUrlForImportFile(this.solfacId), authToken: `Bearer ${Cookie.get('access_token')}`, maxFileSize: 10*1024*1024 });

        this.uploader.onCompleteItem = (response:any) => {
            var json = JSON.parse(response);

            var file = json.data;
            this.files.push({ id: file.id, name: file.name, creationDate: file.creationDate });
        };

        this.uploader.onSuccessItem = (item: any) => {
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
        });
    }

    getCertificatesRelated(){
        this.getCertificatesRelatedSubscrip = this.solfacService.getCertificatesRelated(this.solfacId).subscribe(d => {
            this.certificatesRelated = d;
        });
    }

    clearSelectedFile(){
        this.uploader.clearQueue();
  
        this.selectedFile.nativeElement.value = '';
    }

    exportFile(file){
        this.solfacService.downloadFile(file.id).subscribe(response => {
            FileSaver.saveAs(response, file.name);
        });
    }
 
    showPdf(file){
        this.onViewPdf.emit(file);
    }

    canUploadFiles(){
        return this.menuService.hasFunctionality("SOLFA", "AFILE");
    }

    deleteFile(){
        this.confirmModal.hide();
        this.messageService.showLoading();

        this.solfacService.deleteFile(this.fileId).subscribe(() => {
            this.files.splice(this.index, 1);
            this.fileId = null;
            this.index = null;
            this.messageService.closeLoading();
        },
        () => {
                this.messageService.closeLoading();
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
        });
    }

    exportCertificate(certificate){
        this.certificateService.exportFile(certificate.fileId).subscribe(file => {
            FileSaver.saveAs(file, certificate.fileName);
        });
    }

    deleteCertificate(){
        this.getCertificateAvailableSubscrip = this.solfacService.deleteCertificate(this.solfacId, this.certificateId).subscribe(() => {
            this.certificatesRelated.splice(this.index, 1);
            this.certificates.push({ id: this.certificateId, text: this.certificateName });
            $("#certificate").select2('data', { id: this.certificateId.toString(), text: this.certificateName });
        },
          () => { },
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
        () => { },
        () => this.messageService.closeLoading());
      }
}