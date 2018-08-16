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
                private activatedRoute: ActivatedRoute,
                public menuService: MenuService,
                private messageService: MessageService){}

    ngOnInit(): void {
        var data = <any>JSON.stringify(this.activatedRoute.snapshot.data);
        var dataJson = JSON.parse(data);
        if(dataJson) this.fromProfile = dataJson.fromProfile;

        if(this.fromProfile){
            const userInfo = UserInfoService.getUserInfo();

            if(userInfo && userInfo.employeeId && userInfo.name){
                this.model.employeeId = userInfo.employeeId;
                this.userApplicantName = userInfo.name;
            }
        } else {
            this.getEmployees();
        }

        this.getManagers();
        this.getLicenceTypes();
        this.getSectors();
        this.licenseTypeSetOnChange();
    }

    ngOnDestroy(): void {
        if(this.addSubscrip) this.addSubscrip.unsubscribe();
        if(this.getManagersSubscrip) this.getManagersSubscrip.unsubscribe();
        if(this.getLicenseTypeSubscrip) this.getLicenseTypeSubscrip.unsubscribe();
        if(this.getSectorsSubscrip) this.getSectorsSubscrip.unsubscribe();
        if(this.deleteFileSubscrip) this.deleteFileSubscrip.unsubscribe();
    }

    back(){
        if(!this.menuService.userIsRrhh){
            this.router.navigate(['/profile/' + this.model.employeeId]);
        }
    }

    configUploader(){
        this.uploader = new FileUploader({url: this.licenseService.getUrlForImportFile(this.model.id), authToken: `Bearer ${Cookie.get('access_token')}`, maxFileSize: 10*1024*1024 });

        this.uploader.onCompleteItem = (item:any, response:any, status:any, headers:any) => {
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

    add(){
        if(!this.fromProfile){
            this.model.employeeId = $( "#employeeId" ).val();
        }
        
        this.messageService.showLoading();

        this.addSubscrip = this.licenseService.add(this.model).subscribe(data => {
            this.messageService.closeLoading();
            this.model.id = data.data;
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
            this.resources = data;
        },
        () => {
                this.messageService.closeLoading();
            });
    }

    getManagers(){
        this.getManagersSubscrip = this.licenseService.getAuthorizers().subscribe(data => {
            this.managers = data;
            this.model.managerId = 0;
        });
    }

    getLicenceTypes(){
        this.getLicenseTypeSubscrip = this.licenseService.getLicenceTypes().subscribe(data => {
            this.licensesTypesOptions = data;
            this.licensesTypes = data.optionsWithPayment;
            this.model.typeId = 0;
        });
    }

    getSectors(){
        this.getSectorsSubscrip = this.licenseService.getSectors().subscribe(data => {
            this.sectors = data;
            this.model.sectorId = 0;
        });
    }

    licenseTypeSetOnChange(){
        $( "#licensesTypeId" ).change(() => {
            this.model.typeId = $( "#licensesTypeId" ).val();
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
        this.model.managerId = 0;
        this.model.sectorId = 0;
        this.model.startDate = null;
        this.model.endDate = null;
        this.model.comments = "";
        this.model.daysQuantity = 0;
        this.model.examDescription = "";
        this.model.final = false;
        this.model.parcial = false;
        this.model.hasCertificate = false;
        this.model.id = null;
        this.model.withPayment = true;
        this.model.typeId = 0;

        this.startDate.clean();
        this.endDate.clean();
    }
} 