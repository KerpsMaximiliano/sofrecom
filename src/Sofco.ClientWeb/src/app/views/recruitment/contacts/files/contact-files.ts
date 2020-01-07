import { Component, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { ApplicantService } from "app/services/recruitment/applicant.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import * as FileSaver from "file-saver";
import { FileUploader } from "ng2-file-upload";
import { AuthService } from "app/services/common/auth.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { FileService } from "app/services/common/file.service";

@Component({
    selector: 'contact-files',
    templateUrl: './contact-files.html',
})
export class ContactFileComponent implements OnDestroy {   
    files: any[] = new Array();
    applicantId: number;

    @ViewChild('selectedFile') selectedFile: any;
    public uploader: FileUploader = new FileUploader({url:""});
    
    getFileSubscrip: Subscription;

    constructor(private applicantService: ApplicantService, 
        private messageService: MessageService,
        private fileService: FileService,
        private authService: AuthService,
        private dataTableService: DataTableService){
    }
    
    ngOnDestroy(): void {
        if (this.getFileSubscrip) this.getFileSubscrip.unsubscribe();
    }

    init(id){
        this.applicantId = id;
        this.getFiles();
        this.uploaderConfig();
    }

    getFiles(){
        this.files = [];

        this.getFileSubscrip = this.applicantService.getFiles(this.applicantId).subscribe(response => {
          this.files = response.data;
    
          this.initGrid();
        });
    }

    initGrid() {
        var columns = [0, 1, 2];

        var options = {
            selector: "#filesTable",
            columns: columns,
            columnDefs: [ { "aTargets": [1], "sType": "date-uk" }],
        };

        this.dataTableService.destroy(options.selector);
        this.dataTableService.initialize(options);
    }

    export(item){
        this.messageService.showLoading();

        this.applicantService.getFile(item.id).subscribe(response => {
            this.messageService.closeLoading();

            FileSaver.saveAs(response, item.name);
        },
        () => this.messageService.closeLoading());
    }

    uploaderConfig(){
        this.uploader = new FileUploader({url: this.applicantService.getUrlForImportExcel(this.applicantId),
                                          authToken: 'Bearer ' + Cookie.get('access_token') ,
                                          maxFileSize: 50*1024*1024
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
                
                this.getFiles();
            }

            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    }

    delete(id){
        this.messageService.showConfirm(() => {
            this.messageService.showLoading();

            this.fileService.delete(id).subscribe(response => {
                this.messageService.closeLoading()

                var index = this.files.findIndex(x => x.id == id);
                if(index > -1){
                    this.files.splice(index, 1);
                }
            },
            () => this.messageService.closeLoading());
        });
    }
} 