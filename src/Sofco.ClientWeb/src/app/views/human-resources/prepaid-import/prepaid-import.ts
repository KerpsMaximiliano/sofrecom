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
import { RrhhService } from "app/services/human-resources/rrhh.service";

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
    notifySubscrip: Subscription;

    public uploader: FileUploader = new FileUploader({url:""});

    public yearId: number;
    public monthId: number;
    public prepaidId: number;

    public years: any[] = new Array();
    public months: any[] = new Array();
    public prepaids: any[] = new Array();
    public itemsDashboard: any[] = new Array();

    public mustSyncWithTiger: boolean = false;

    constructor(private utilsService: UtilsService,
                private i18nService: I18nService,
                private prepaidService: PrepaidService,
                private rrhhService: RrhhService,
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
        if(this.notifySubscrip) this.notifySubscrip.unsubscribe();
    }

    dateChange(){
        this.getDashboard();
    }

    getDashboard(){
        this.itemsDashboard = [];

        if(!this.yearId || !this.monthId || this.yearId <= 0 || this.monthId <= 0) return;

        this.getDashboardSubscrip = this.prepaidService.getDashboard(this.yearId, this.monthId).subscribe(response => {
            this.mustSyncWithTiger = response.data.mustSyncWithTiger;

            this.uploaderConfig();

            if(response && response.data && response.data.dashboard.length > 0){
                this.itemsDashboard = response.data.dashboard;
            }
        });
    }

    canSyncSocialCharges(){
        if(!this.yearId || !this.monthId || this.yearId <= 0 || this.monthId <= 0 || !this.prepaids || !this.itemsDashboard) return false;

        return this.yearId > 0 && this.monthId > 0;
    }

    syncSocialCharges(){
        if(!this.yearId || !this.monthId || this.yearId <= 0 || this.monthId <= 0) return;

        this.messageService.showLoading();

        this.notifySubscrip = this.rrhhService.syncSocialCharges(this.yearId, this.monthId).subscribe(response => {
          this.messageService.closeLoading();

          if(this.mustSyncWithTiger){
            this.mustSyncWithTiger = false;
            this.uploaderConfig();
          }
        });
    }

    notifyToRrhh(){
        if(!this.yearId || !this.monthId || this.yearId <= 0 || this.monthId <= 0) return;

        this.messageService.showLoading();

        this.notifySubscrip = this.prepaidService.notifyToRrhh(this.yearId, this.monthId).subscribe(response => {
          this.messageService.closeLoading();
        });
    }

    canNotify(){
        if(!this.yearId || !this.monthId || this.yearId <= 0 || this.monthId <= 0 || !this.prepaids || !this.itemsDashboard) return false;

        return this.yearId > 0 && this.monthId > 0 && this.itemsDashboard.length == this.prepaids.length;
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

            var dataJson = JSON.parse(response);

            this.prepaidId = null;

            if(dataJson.messages){
                this.messageService.showMessages(dataJson.messages);
            }

            if(dataJson.data){
                var itemExist = this.itemsDashboard.findIndex(x => x.prepaid == dataJson.data.prepaid);

                if(itemExist >= 0){
                    this.itemsDashboard.splice(itemExist, 1);
                }

                this.itemsDashboard.push(dataJson.data);
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
}