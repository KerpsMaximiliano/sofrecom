import { Component, Input, ViewChild, OnDestroy, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { SolfacService } from 'app/services/billing/solfac.service';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { FileUploader } from 'ng2-file-upload';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { MessageService } from 'app/services/common/message.service';
import * as FileSaver from "file-saver";
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';

@Component({
  selector: 'solfac-attachments',
  templateUrl: './solfac-attachments.component.html',
  styleUrls: ['./solfac-attachments.component.scss']
})
export class SolfacAttachmentsComponent implements OnInit, OnDestroy {

    @Input() solfacId: number;
    @ViewChild('selectedFile') selectedFile: any;
    @ViewChild('confirmDeleteFileModal') confirmModal;
    
    files: any[] = new Array<any>();
    fileId: number;
    index: number;
    
    getAttachmentsSubscrip: Subscription;

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
                private errorHandlerService: ErrorHandlerService) {
    }

    ngOnInit() {
        this.getAttachments();

        this.uploader = new FileUploader({url: this.solfacService.getUrlForImportFile(this.solfacId), authToken: Cookie.get('access_token'), maxFileSize: 10*1024*1024 });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            var json = JSON.parse(response);

            if(json.messages) this.messageService.showMessages(json.messages);

            var file = json.data;
            this.files.push({ id: file.id, name: file.name, creationDate: file.creationDate });

            this.clearSelectedFile();
        };
    }

    ngOnDestroy(){
        if(this.getAttachmentsSubscrip) this.getAttachmentsSubscrip.unsubscribe();
    }

    getAttachments(){
        this.getAttachmentsSubscrip = this.solfacService.getAttachments(this.solfacId).subscribe(d => {
            this.files = d;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    }

    exportExcel(file){
        this.solfacService.getFile(file.id).subscribe(response => {
            FileSaver.saveAs(response, file.name);
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    deleteFile(){
        this.solfacService.deleteFile(this.fileId).subscribe(response => {
            if(response.messages) this.messageService.showMessages(response.messages);
            this.files.splice(this.index, 1);

            this.confirmModal.hide();
            this.fileId = null;
            this.index = null;
        },
        err => {
            this.confirmModal.hide();
            this.errorHandlerService.handleErrors(err)
        });
    }

    showConfirm(fileId, index){
        this.fileId = fileId;
        this.index = index;
        this.confirmModal.show();
    }
}