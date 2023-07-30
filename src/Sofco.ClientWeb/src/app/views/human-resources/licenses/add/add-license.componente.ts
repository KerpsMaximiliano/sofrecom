import { Component, OnDestroy, ViewChild, OnInit } from "@angular/core";
import { Router, ActivatedRoute } from "@angular/router";
import { Subscription } from "rxjs";

// * Services.
import { MenuService } from "../../../../services/admin/menu.service";
import { EmployeeService } from "../../../../services/allocation-management/employee.service";
import { AuthService } from "../../../../services/common/auth.service";
import { UserInfoService } from "../../../../services/common/user-info.service";
import { MessageService } from "../../../../services/common/message.service";
import { LicenseService } from "../../../../services/human-resources/licenses.service";

// * Interfaces.
import { License } from "../../../../models/rrhh/license";

// * Others.
import { Cookie } from "ng2-cookies/ng2-cookies";
import { FileUploader } from "ng2-file-upload";

// * Components.
import { Ng2ModalConfig } from "../../../../components/modal/ng2modal-config";

declare var $: any;

@Component({
  selector: "add-license",
  templateUrl: "./add-license.component.html",
  styleUrls: ["./add-license.component.scss"],
})
export class AddLicenseComponent implements OnInit, OnDestroy {
  @ViewChild("selectedFile") selectedFile: any;

  addSubscrip: Subscription;
  getEmployeesSubscrip: Subscription;
  getManagersSubscrip: Subscription;
  getLicenseTypeSubscrip: Subscription;
  getSectorsSubscrip: Subscription;
  deleteFileSubscrip: Subscription;
  userSubscrip: Subscription;

  public uploader: FileUploader = new FileUploader({ url: "" });
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

  @ViewChild("confirmDeleteFileModal") confirmModal;
  public confirmModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
    "ACTIONS.confirmTitle",
    "confirmDeleteFileModal",
    true,
    true,
    "ACTIONS.ACCEPT",
    "ACTIONS.cancel"
  );

  @ViewChild("startDate") startDate;
  @ViewChild("endDate") endDate;

  constructor(
    private licenseService: LicenseService,
    private employeeService: EmployeeService,
    private authService: AuthService,
    public menuService: MenuService,
    private messageService: MessageService,
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {}

  ngOnInit(): void {
    var data = <any>JSON.stringify(this.activatedRoute.snapshot.data);
    var dataJson = JSON.parse(data);
    if (dataJson) this.fromProfile = dataJson.fromProfile;

    if (this.fromProfile) {
      this.initUserInfo();
    } else {
      this.getEmployees();
      this.model.employeeId = "0";
    }

    this.getLicenceTypes();
  }

  ngOnDestroy(): void {
    if (this.addSubscrip) this.addSubscrip.unsubscribe();
    if (this.getManagersSubscrip) this.getManagersSubscrip.unsubscribe();
    if (this.getLicenseTypeSubscrip) this.getLicenseTypeSubscrip.unsubscribe();
    if (this.getSectorsSubscrip) this.getSectorsSubscrip.unsubscribe();
    if (this.deleteFileSubscrip) this.deleteFileSubscrip.unsubscribe();
    if (this.userSubscrip) this.userSubscrip.unsubscribe();
  }

  public back(): void {
    if (!this.menuService.userIsRrhh) {
      this.router.navigate(["/profile/" + this.model.employeeId]);
    }
  }

  public add(): void {
    this.messageService.showLoading();

    // Restable la hora, minutos y segundos a 00:00:00.000.
    if (this.model.startDate)
      this.model.startDate = this.dateFormatter(this.model.startDate);
    if (this.model.endDate) {
      this.model.endDate = this.dateFormatter(this.model.endDate);
    }

    this.addSubscrip = this.licenseService.add(this.model).subscribe(
      (res) => {
        this.messageService.closeLoading();
        this.model.id = res.data.id;
        this.model.managerId = res.data.managerId;
        this.model.managerDesc = res.data.managerDesc;
        this.updateStorage();
        this.configUploader();
      },
      () => {
        this.messageService.closeLoading();
      }
    );
  }

  public withPaymentChange(event: any): void {
    if (!event) {
      this.licensesTypes = this.licensesTypesOptions.optionsWithoutPayment;
      this.model.typeId = 12;
      this.model.parcial = false;
      this.model.final = false;
      this.model.examDescription = null;
    } else {
      this.licensesTypes = this.licensesTypesOptions.optionsWithPayment;
      this.model.typeId = 1;
      this.model.comments = null;
    }
  }

  public clearSelectedFile(): void {
    this.uploader.clearQueue();
    this.selectedFile.nativeElement.value = "";
  }

  public openConfirmModal(fileId: any, index: any): void {
    this.fileIdToDelete = fileId;
    this.indexToDelete = index;
    this.confirmModal.show();
  }

  public deleteFile(): void {
    this.deleteFileSubscrip = this.licenseService
      .deleteFile(this.fileIdToDelete)
      .subscribe(
        () => {
          this.files.splice(this.indexToDelete, 1);
        },
        () => {},
        () => this.confirmModal.hide()
      );
  }

  public refresh(): void {
    this.model = new License();

    if (this.fromProfile) {
      this.initUserInfo();
    } else {
      this.model.employeeId = "0";
      this.model.typeId = 0;
    }
  }

  private initUserInfo(): void {
    const userInfo = UserInfoService.getUserInfo();

    if (userInfo && userInfo.employeeId && userInfo.name) {
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

  private configUploader(): void {
    this.uploader = new FileUploader({
      url: this.licenseService.getUrlForImportFile(this.model.id),
      authToken: `Bearer ${Cookie.get("access_token")}`,
      maxFileSize: 10 * 1024 * 1024,
    });

    this.uploader.onCompleteItem = (
      item: any,
      response: any,
      status: any,
      headers: any
    ) => {
      if (status == 401) {
        this.authService.refreshToken().subscribe((token) => {
          this.messageService.closeLoading();

          if (token) {
            this.clearSelectedFile();
            this.messageService.showErrorByFolder("common", "fileMustReupload");
            this.configUploader();
          }
        });

        return;
      }

      var json = JSON.parse(response);

      if (json.messages) this.messageService.showMessages(json.messages);

      var file = json.data;
      this.files.push({ id: file.id, name: file.fileName });
    };

    this.uploader.onSuccessItem = (item: any) => {
      item.remove();
      this.selectedFile.nativeElement.value = "";
    };
  }

  private employeeChange(): void {
    this.messageService.showLoading();

    this.userSubscrip = this.employeeService
      .getInfo(this.model.employeeId)
      .subscribe(
        (response) => {
          this.messageService.closeLoading();

          if (response.data) {
            this.model.managerId = response.data.managerId;
            this.model.managerDesc = response.data.managerDesc;
            this.model.sectorId = response.data.sectorId;
            this.model.sectorDesc = response.data.sectorDesc;
            this.model.authorizerId = response.data.authorizerId;
            this.model.authorizerDesc = response.data.authorizerDesc;

            this.checkMissingManager();
          }
        },
        (error: any) => {
          this.messageService.closeLoading();
          this.model.managerId = null;
          this.model.managerDesc = null;
          this.model.sectorId = null;
          this.model.sectorDesc = null;
          this.model.authorizerId = null;
          this.model.authorizerDesc = null;
        }
      );
  }

  private checkMissingManager(): void {
    !this.model.managerId || this.model.managerId <= 0
      ? (this.missingManager = true)
      : (this.missingManager = false);
  }

  private getEmployees(): void {
    this.messageService.showLoading();

    this.getEmployeesSubscrip = this.employeeService.getAll().subscribe(
      (data) => {
        this.messageService.closeLoading();
        this.resources = data.map((item: any) => {
          return { id: item.id, text: item.name };
        });
      },
      () => {
        this.messageService.closeLoading();
      }
    );
  }

  private getLicenceTypes(): void {
    this.getLicenseTypeSubscrip = this.licenseService
      .getLicenceTypes()
      .subscribe((data: any) => {
        this.licensesTypesOptions = data;
        this.licensesTypes = data.optionsWithPayment;
        this.model.typeId = 0;
      });
  }

  private updateStorage(): void {
    const userInfo = UserInfoService.getUserInfo();
    userInfo.managerId = this.model.managerId;
    userInfo.managerDesc = this.model.managerDesc;
    UserInfoService.setUserInfo(userInfo);
  }

  /**
   * Cambia la hora, minutos y segundos de una fecha con formato: ISO 8601 y devuelve la nueva fecha.
   * @param date Licencia desde/hasta con formato: ISO 8601 UTF.
   * @returns Licencia desde/hasta con formato: ISO 8601 c/UTF = 00:00:00.000.
   */
  private dateFormatter(date: Date): Date {
    let format = new Date(date);

    format.setHours(0, 0, 0, 0);

    console.log(format);

    return format;
  }
}
