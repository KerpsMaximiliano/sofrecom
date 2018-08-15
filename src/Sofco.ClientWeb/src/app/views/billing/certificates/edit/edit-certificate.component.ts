import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { MessageService } from "../../../../services/common/message.service";
import { Subscription } from "rxjs";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies/ng2-cookies";
import * as FileSaver from "file-saver";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
import { CertificatesService } from "../../../../services/billing/certificates.service";


@Component({
    selector: 'edit-certificate',
    templateUrl: './edit-certificate.component.html',
    styleUrls: ['./edit-certificate.component.scss']
})
export class EditCertificateComponent implements OnInit, OnDestroy {

    @ViewChild('form') form;
    @ViewChild('selectedFile') selectedFile: any;
    @ViewChild('pdfViewer') pdfViewer;

    updateSubscrip: Subscription;
    getSubscrip: Subscription;
    paramsSubscrip: Subscription;
    getByIdSubscrip: Subscription;

    public uploader: FileUploader = new FileUploader({url:""});
    public showUploader: boolean = false;

    public fileName: string;
    public creationDate: string;
    public fileId: number;

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
                private activatedRoute: ActivatedRoute,
                private messageService: MessageService){
    }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.messageService.showLoading();

            this.getByIdSubscrip = this.certificateService.getById(params['id']).subscribe(data => {
                this.messageService.closeLoading();
                this.form.model = data;

                this.uploaderConfig();
            },
            () => this.messageService.closeLoading());
        });
    }

    ngOnDestroy(): void {
        if(this.updateSubscrip) this.updateSubscrip.unsubscribe();
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
        if(this.getByIdSubscrip) this.getByIdSubscrip.unsubscribe();
    }

    update() {
        this.messageService.showLoading();

        var client = this.form.customers.find(x => x.id == this.form.model.clientExternalId);
        this.form.model.clientExternalName = client.text;

        this.updateSubscrip = this.certificateService.update(this.form.model).subscribe(
            () => {
                this.messageService.closeLoading();
            },
            () => this.messageService.closeLoading());
    }

    uploaderConfig(){
        this.uploader = new FileUploader({url: this.certificateService.getUrlForImportFile(this.form.model.id),
                                          authToken: 'Bearer ' + Cookie.get('access_token') ,
                                          maxFileSize: 10*1024*1024
                                        });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            var dataJson = JSON.parse(response);
            
            if(dataJson){
                this.form.model.fileName = dataJson.data.fileName;
                this.form.model.creationDate = new Date(dataJson.data.creationDate).toLocaleDateString();
                this.form.model.fileId = dataJson.data.id;
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

    viewFile(){
        if(this.form.model.fileName.endsWith('.pdf')){
            this.certificateService.getFile(this.form.model.fileId).subscribe(response => {
                this.pdfViewer.renderFile(response.data);
            });
        }
    }
}  