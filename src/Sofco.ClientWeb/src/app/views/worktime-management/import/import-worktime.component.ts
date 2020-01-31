import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { FileUploader } from "ng2-file-upload";
import { Subscription } from "rxjs/Subscription";
import { MessageService } from "app/services/common/message.service";
import { WorktimeService } from "app/services/worktime-management/worktime.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { I18nService } from "../../../services/common/i18n.service";
import * as FileSaver from "file-saver";
import { DataTableService } from "../../../services/common/datatable.service";
import { AuthService } from "../../../services/common/auth.service";
import { UtilsService } from "app/services/common/utils.service";

@Component({
    selector: 'import-worktime',
    templateUrl: './import-worktime.component.html'
})
export class ImportWorkTimesComponent implements OnInit, OnDestroy {
    @ViewChild('selectedFile') selectedFile: any;

    getSubscrip: Subscription;
    getMonthsSubscrip: Subscription;

    public uploader: FileUploader = new FileUploader({url:""});

    public analyticId: number;
    public closeMonthId: number;

    public analytics: any[] = new Array();
    public errors: any[] = new Array();
    public months: any[] = new Array<any>();

    public showSuccess: boolean = false;
 
    constructor(private messageService: MessageService, 
                private authService: AuthService,
                private utilsService: UtilsService,
                private dataTableService: DataTableService,
                private i18nService: I18nService,
                private worktimeService: WorktimeService){    
    }
  
    ngOnInit(): void {
        this.messageService.showLoading();

        this.getMonths();

        this.getSubscrip = this.worktimeService.getAnalytics().subscribe(data => {
            this.messageService.closeLoading();
            this.analytics = data;
        });
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    getMonths(){
        this.getMonthsSubscrip = this.utilsService.getCloseMonths().subscribe(data => this.months = data);
    }

    uploaderConfig(){
        this.uploader = new FileUploader({url: this.worktimeService.getUrlForImportFile(this.analyticId),
            authToken: 'Bearer ' + Cookie.get('access_token') ,
            maxFileSize: 10*1024*1024,
            allowedMimeType: ['application/vnd.ms-excel','application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'],
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
                this.errors = dataJson.data;

                if(this.errors.length > 0){
                    var options = { 
                        selector: "#errorsTable",
                        withOutSorting: true
                    };
            
                    this.dataTableService.destroy(options.selector); 
                    this.dataTableService.initialize(options);
                }
            }

            if(this.errors.length == 0 && dataJson.messages.filter(x => x.type == 1).length == 0){
                this.showSuccess = true;
            }

            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file)=> { file.withCredentials = false; };
    }

    upload(){
        this.showSuccess = false;
        this.messageService.showLoading();
        this.uploader.uploadAll();
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

        this.worktimeService.exportTemplate(this.analyticId, this.closeMonthId).subscribe(file => {
            this.messageService.closeLoading();

            var analytic = this.analytics.find(x => x.id == this.analyticId);
            var period = this.months.find(x => x.id == this.closeMonthId);

            FileSaver.saveAs(file, `${analytic.text} - ${period.text}.xlsx`);
        },
        () => this.messageService.closeLoading());
    }
}