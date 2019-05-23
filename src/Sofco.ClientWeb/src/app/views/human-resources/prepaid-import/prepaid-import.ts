import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { FileUploader } from "ng2-file-upload";
import { UtilsService } from "app/services/common/utils.service";
import { I18nService } from "app/services/common/i18n.service";
import { MessageService } from "app/services/common/message.service";
import { AuthService } from "app/services/common/auth.service";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { PrepaidService } from "app/services/human-resources/prepaid.service";

@Component({
    selector: 'prepaid-import',
    templateUrl: './prepaid-import.html'
})
export class PrepaidImportComponent implements OnInit, OnDestroy {
    @ViewChild('selectedFile') selectedFile: any;

    getYearsSubscrip: Subscription;
    getMonthsSubscrip: Subscription;
    getPrepaidsSubscrip: Subscription;
    getDashboardSubscrip: Subscription;

    public uploader: FileUploader = new FileUploader({url:""});

    public yearId: number;
    public monthId: number;
    public prepaidId: number;

    public years: any[] = new Array();
    public months: any[] = new Array();
    public prepaids: any[] = new Array();
    public itemsDashboard: any[] = new Array();

    constructor(private utilsService: UtilsService,
                private i18nService: I18nService,
                private prepaidService: PrepaidService,
                private employeeService: EmployeeService,
                private messageService: MessageService, 
                private authService: AuthService,){

    }

    ngOnInit(): void {
        this.getYearsSubscrip = this.utilsService.getYears().subscribe(data => {
            this.years = data;
        });

        this.getMonthsSubscrip = this.utilsService.getMonths().subscribe(data => {
            this.months = data.map(item => {
                item.text = this.i18nService.translateByKey(item.text);
                return item;
            });
        });

        this.getPrepaidsSubscrip = this.utilsService.getPrepaids().subscribe(data => {
            this.prepaids = data;
        });
    }

    ngOnDestroy(): void {
        if(this.getYearsSubscrip) this.getYearsSubscrip.unsubscribe();
        if(this.getMonthsSubscrip) this.getMonthsSubscrip.unsubscribe();
        if(this.getPrepaidsSubscrip) this.getPrepaidsSubscrip.unsubscribe();
        if(this.getDashboardSubscrip) this.getDashboardSubscrip.unsubscribe();
    }

    dateChange(){
        this.getDashboard();
        this.uploaderConfig();
    }

    getDashboard(){
        this.itemsDashboard = [];

        if(!this.yearId || !this.monthId || this.yearId <= 0 || this.monthId <= 0) return;

        this.getDashboardSubscrip = this.prepaidService.get(this.yearId, this.monthId).subscribe(response => {
            if(response && response.data && response.data.length > 0){
                this.itemsDashboard = response.data;
            }
        });
    }

    uploaderConfig(){
        if(!this.yearId || !this.monthId || !this.prepaidId || this.yearId <= 0 || this.monthId <= 0 || this.prepaidId <= 0) return;

        this.uploader = new FileUploader({url: this.employeeService.getUrlForImportFile(this.yearId, this.monthId, this.prepaidId),
            authToken: 'Bearer ' + Cookie.get('access_token') ,
            maxFileSize: 50*1024*1024,
            allowedMimeType: ['application/vnd.ms-excel','application/vnd.openxmlformats-officedocument.spreadsheetml.sheet', 'text/csv'],
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
}