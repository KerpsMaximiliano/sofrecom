import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { MessageService } from "../../../../services/common/message.service";
import { Subscription } from "rxjs";
import { FileUploader } from "ng2-file-upload";
import { LicenseService } from "../../../../services/human-resources/licenses.service";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { MenuService } from "../../../../services/admin/menu.service";
import { License } from "../../../../models/rrhh/license";
import { Cookie } from "ng2-cookies/ng2-cookies";
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";
import { UserInfoService } from "../../../../services/common/user-info.service";
import { AuthService } from "../../../../services/common/auth.service";

declare var $: any;

@Component({
    selector: 'add-license',
    templateUrl: './add-license.component.html'
})
export class AddLicenseComponent implements OnInit, OnDestroy {

    @ViewChild('selectedFile') selectedFile: any;

    addSubscrip: Subscription;
    getEmployeesSubscrip: Subscription;
    getManagersSubscrip: Subscription;
    getLicenseTypeSubscrip: Subscription;
    getSectorsSubscrip: Subscription;
    deleteFileSubscrip: Subscription;
    userSubscrip: Subscription;

    public uploader: FileUploader = new FileUploader({url:""});
    public showUploader: boolean = false;

    public model: License = new License();

    public resources: any[] = new Array();
    public managers: any[] = new Array();
    public licensesTypesOptions: any;
    public sectors: any[] = new Array();
    public licensesTypes: any[] = new Array();
    public files: any[] = new Array();

    public fromProfile: boolean = false;
    public missingManager: boolean = false;
    public userApplicantName: string;
    public fileIdToDelete: number;
    public indexToDelete: number;

    @ViewChild('confirmDeleteFileModal') confirmModal;
    public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "ACTIONS.confirmTitle",
        "confirmDeleteFileModal",
        true,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.cancel"
    );

    @ViewChild('startDate') startDate;
    @ViewChild('endDate') endDate;

    constructor(private licenseService: LicenseService,
                private employeeService: EmployeeService,
                private router: Router,
                private authService: AuthService,
                private activatedRoute: ActivatedRoute,
                public menuService: MenuService,
                private messageService: MessageService){}

    ngOnInit(): void {
        var data = <any>JSON.stringify(this.activatedRoute.snapshot.data);
        var dataJson = JSON.parse(data);
        if(dataJson) this.fromProfile = dataJson.fromProfile;

        if(this.fromProfile){
            this.initUserInfo();            
        } else {
            this.getEmployees();
            this.model.employeeId = '0';
        }

        this.getLicenceTypes();
    }
 
    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
        if(this.getManagersSubscrip) this.getManagersSubscrip.unsubscribe();
        if(this.getLicenseTypeSubscrip) this.getLicenseTypeSubscrip.unsubscribe();
        if(this.getSectorsSubscrip) this.getSectorsSubscrip.unsubscribe();
        if(this.deleteFileSubscrip) this.deleteFileSubscrip.unsubscribe();
        if(this.userSubscrip) this.userSubscrip.unsubscribe();
    }

    initUserInfo(){
        const userInfo = UserInfoService.getUserInfo();

        if(userInfo && userInfo.employeeId && userInfo.name){
            this.model.employeeId = userInfo.employeeId;
            this.model.managerId = userInfo.managerId;
            this.model.managerDesc = userInfo.managerDesc;
            this.model.sectorId = userInfo.sectorId;
            this.model.sectorDesc = userInfo.sectorDesc;
            this.model.authorizerId = userInfo.authorizerId;
            this.model.authorizerDesc = userInfo.authorizerDesc;

            this.userApplicantName = userInfo.name;

            this.checkMissingManager();
        }
    }

    back(){
        if(!this.menuService.userIsRrhh){
            this.router.navigate(['/profile/' + this.model.employeeId]);
        }
    }

    configUploader(){
        this.uploader = new FileUploader({url: this.licenseService.getUrlForImportFile(this.model.id), authToken: `Bearer ${Cookie.get('access_token')}`, maxFileSize: 10*1024*1024 });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
            if(status == 401){
                this.authService.refreshToken().subscribe(token => {
                    this.messageService.closeLoading();

                    if(token){
                        this.clearSelectedFile();
                        this.messageService.showErrorByFolder('common', 'fileMustReupload');
                        this.configUploader();
                    }
                });

                return;
            }

            var json = JSON.parse(response);

            if(json.messages) this.messageService.showMessages(json.messages);
            
            var file = json.data;
            this.files.push({ id: file.id, name: file.fileName });
        };

        this.uploader.onSuccessItem = (item: any) => {
            item.remove();
            this.selectedFile.nativeElement.value = '';
        };
    }

    employeeChange(){
        this.messageService.showLoading();

        this.userSubscrip = this.employeeService.getInfo(this.model.employeeId).subscribe(
            response => {
                this.messageService.closeLoading();

                if(response.data){
                    this.model.managerId = response.data.managerId;
                    this.model.managerDesc = response.data.managerDesc;
                    this.model.sectorId = response.data.sectorId;
                    this.model.sectorDesc = response.data.sectorDesc;
                    this.model.authorizerId = response.data.authorizerId;
                    this.model.authorizerDesc = response.data.authorizerDesc;

                    this.checkMissingManager();
                }
            },
            error => {
                this.messageService.closeLoading();
                this.model.managerId = null;
                this.model.managerDesc = null;
                this.model.sectorId = null;
                this.model.sectorDesc = null;
                this.model.authorizerId = null;
                this.model.authorizerDesc = null;
            });
    }

    checkMissingManager(){
        if(!this.model.managerId || this.model.managerId <= 0){
            this.missingManager = true;
        }
        else{
            this.missingManager = false;
        }
    }

    add(){
        this.messageService.showLoading();

        this.addSubscrip = this.licenseService.add(this.model).subscribe(res => {
            this.messageService.closeLoading();
            this.model.id = res.data.id;
            this.model.managerId = res.data.managerId;
            this.model.managerDesc = res.data.managerDesc;
            this.updateStorage();
            this.configUploader();
        },
        () => {
                this.messageService.closeLoading();
            });
    }

    getEmployees(){
        this.messageService.showLoading();

        this.getEmployeesSubscrip = this.employeeService.getAll().subscribe(data => {
            this.messageService.closeLoading();
            this.resources = data.map(item => { 
                return { id: item.id, text: item.name };
             });
        },
        () => {
            this.messageService.closeLoading();
        });
    }

    getLicenceTypes(){
        this.getLicenseTypeSubscrip = this.licenseService.getLicenceTypes().subscribe(data => {
            this.licensesTypesOptions = data;
            this.licensesTypes = data.optionsWithPayment;
            this.model.typeId = 0;
        });
    }

    withPaymentChange(event){
        if(!event){
            this.licensesTypes = this.licensesTypesOptions.optionsWithoutPayment;
            this.model.typeId = 12;
            this.model.parcial = false;
            this.model.final = false;
            this.model.examDescription = null;
        }
        else{
            this.licensesTypes = this.licensesTypesOptions.optionsWithPayment;
            this.model.typeId = 1;
            this.model.comments = null;
        }
    }

    clearSelectedFile(){
        this.uploader.clearQueue();
        this.selectedFile.nativeElement.value = '';
    }

    openConfirmModal(fileId, index){
        this.fileIdToDelete = fileId;
        this.indexToDelete = index;
        this.confirmModal.show();
    }

    deleteFile(){
        this.deleteFileSubscrip = this.licenseService.deleteFile(this.fileIdToDelete).subscribe(() => {
            this.files.splice(this.indexToDelete, 1);
        },
          () => { },
         () => this.confirmModal.hide());
    }

    refresh(){
        this.model = new License();

        if(this.fromProfile){
            this.initUserInfo();            
        } else {
            this.model.employeeId = '0';
        }
    }

    updateStorage() {
        const userInfo = UserInfoService.getUserInfo();
        userInfo.managerId = this.model.managerId;
        userInfo.managerDesc = this.model.managerDesc;
        UserInfoService.setUserInfo(userInfo);
    }
} 
