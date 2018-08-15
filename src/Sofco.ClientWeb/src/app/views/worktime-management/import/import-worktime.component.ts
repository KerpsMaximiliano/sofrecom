import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { FileUploader } from "ng2-file-upload";
import { Subscription } from "rxjs/Subscription";
import { MessageService } from "app/services/common/message.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { WorktimeService } from "app/services/worktime-management/worktime.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { I18nService } from "../../../services/common/i18n.service";
import * as FileSaver from "file-saver";

@Component({
    selector: 'import-worktime',
    templateUrl: './import-worktime.component.html'
})
export class ImportWorkTimesComponent implements OnInit, OnDestroy {
    @ViewChild('selectedFile') selectedFile: any;

    getSubscrip: Subscription;

    public uploader: FileUploader = new FileUploader({url:""});

    public analyticId: number = 0;

    public analytics: any[] = new Array();
    public errors: any[] = new Array();

    public showSuccess: boolean = false;

    constructor(private analyticService: AnalyticService,
                private messageService: MessageService, 
                private i18nService: I18nService,
                private worktimeService: WorktimeService){    
    }

    ngOnInit(): void {
        this.messageService.showLoading();

        this.getSubscrip = this.analyticService.getByManager().subscribe(response => {
            this.messageService.closeLoading();
            this.analytics = response.data;
        });
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        this.i18nService.translate
    }

    uploaderConfig(){
        this.uploader = new FileUploader({url: this.worktimeService.getUrlForImportFile(this.analyticId),
                                          authToken: 'Bearer ' + Cookie.get('access_token') ,
                                          maxFileSize: 10*1024*1024,
                                          allowedMimeType: ['application/vnd.ms-excel','application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'],
                                        });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            this.messageService.closeLoading();

            if(status == 401){
                console.log('volver a subir el archivo');
                this.clearSelectedFile();
                return
            }

            var dataJson = JSON.parse(response);
            
            if(dataJson){
                if(dataJson.messages) this.messageService.showMessages(dataJson.messages);
                this.errors = dataJson.data;
            }

            if(this.errors.length == 0){
                this.showSuccess = true;
            }

            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
    }

    upload(){
        this.messageService.showLoading();
        this.uploader.uploadAll()
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    } 

    translateError(msg){
        var split = msg.split('.');
        return this.i18nService.translate(split[0], split[1]);
    }

    exportTemplate(){
        this.messageService.showLoading();

        this.worktimeService.exportTemplate().subscribe(file => {
            this.messageService.closeLoading();
            FileSaver.saveAs(file, "template carga de horas.xlsx");
        },
        () => this.messageService.closeLoading());
    }
}