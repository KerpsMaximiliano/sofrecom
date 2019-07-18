import { Component, OnInit, OnDestroy, ViewChild } from "@angular/core";
import { Subscription } from "rxjs";
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { SalaryAdvancementService } from "app/services/advancement-and-refund/salary-advancement";
import { FileUploader } from "ng2-file-upload";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { AuthService } from "app/services/common/auth.service";

@Component({
    selector: 'salary-advancement',
    templateUrl: './salary-advancement.html'
})
export class SalaryAdvancementComponent implements OnInit, OnDestroy  {
    public model: any[] = new Array();

    getSubscrip: Subscription;

    @ViewChild('selectedFile') selectedFile: any;
    public uploader: FileUploader = new FileUploader({url:""});

    constructor(private messageService: MessageService,
                private datatableService: DataTableService,
                private authService: AuthService,
                private salaryAdvancementService: SalaryAdvancementService){}

    ngOnInit(): void {
        this.getData();
        this.uploaderConfig();
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
    }

    getData(){
        this.messageService.showLoading();

        this.getSubscrip = this.salaryAdvancementService.get().subscribe(response => {
            this.messageService.closeLoading();
            this.model = response.data;

            this.initGrid();
        },
        () => this.messageService.closeLoading());
    }

    initGrid(){
        var columns = [0, 1, 2, 3];
        var title = `Devoluciones de adelantos de sueldo`;

        var params = {
          selector: '#salary-advancement',
          columns: columns,
          title: title,
          withExport: true
        }
  
        this.datatableService.destroy(params.selector);
        this.datatableService.initialize(params);
    }

    
    uploaderConfig(){
        this.uploader = new FileUploader({url: this.salaryAdvancementService.getUrlForImportFile(),
            authToken: 'Bearer ' + Cookie.get('access_token') ,
            maxFileSize: 50*1024*1024,
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

            var dataJson = JSON.parse(response);

            this.getData();

            if(dataJson.messages){
                this.messageService.showMessages(dataJson.messages);
            }

            this.clearSelectedFile();
        };

        this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };
    }

    clearSelectedFile(){
        if(this.uploader.queue.length > 0){
            this.uploader.queue[0].remove();
        }
  
        this.selectedFile.nativeElement.value = '';
    } 
}