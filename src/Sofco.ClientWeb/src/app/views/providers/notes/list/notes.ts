import {
  AfterViewInit,
  Component,
  ElementRef,
  OnInit,
  ViewChild,
} from "@angular/core";
import { Router } from "@angular/router";
import { environment } from "environments/environment";

// * Services.
import { ProvidersService } from "app/services/admin/providers.service";
import { PurchaseOrderService } from "app/services/admin/purchase-order.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";

// * Components
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";

@Component({
  selector: "notes",
  templateUrl: "./notes.html",
  styleUrls: ["./notes.scss"],
})
export class NotesComponent implements OnInit, AfterViewInit {
  private currentTab: number = 1;

  private notesBackup = [];

  public modalProvidersList = [];

  private notesInProcess = [];
  private notesEnded = [];

  public applicants = [];
  public applicantId: number;

  public states2 = environment.REQUEST_NOTE_STATES;
  public stateId: number;

  public providers = [];
  public providerId: number;

  public dateSince: any;
  public dateTo: any;

  public notes = [];
  public noteId: any;

  public analiticas = [];
  public analyticId: number;

  public providersModalConfig: Ng2ModalConfig;

  @ViewChild("tabOne") public tabOne: ElementRef;
  @ViewChild("tabTwo") public tabTwo: ElementRef;
  @ViewChild("providersModal") public providersModal: Ng2ModalConfig;

  constructor(
    private analyticService: AnalyticService,
    private dataTableService: DataTableService,
    private employeeService: EmployeeService,
    private messageService: MessageService,
    private requestNoteService: RequestNoteService,
    private providerService: ProvidersService,
    private purchaseOrderService: PurchaseOrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.providersModalConfig = new Ng2ModalConfig(
      "Proveedores designados",
      "rejectModal",
      false,
      true,
      "",
      "Cerrar"
    );
  }

  ngAfterViewInit(): void {
    // ! this.inicializar() tiene un manejo incorrecto de peticiones.
    this.inicializar();
  }

  // ! Acceso sin control a 'environment'. Apróximado: 8685 veces.
  get currentEnviroment(): typeof environment {
    return environment;
  }

  public changeIcon(): void {
    $("#collapseOne").hasClass("in")
      ? $("#search-icon")
          .toggleClass("fa-caret-down")
          .toggleClass("fa-caret-up")
      : $("#search-icon")
          .toggleClass("fa-caret-up")
          .toggleClass("fa-caret-down");
  }

  public collapse(): void {
    $("#collapseOne").hasClass("in")
      ? $("#collapseOne").removeClass("in")
      : $("#collapseOne").addClass("in");

    this.changeIcon();
  }

  public search(): void {
    if (
      this.dateTo != null &&
      this.dateTo != undefined &&
      this.dateSince != null &&
      this.dateSince != undefined
    ) {
      if (this.dateTo < this.dateSince) {
        this.messageService.showMessage(
          "La fecha hasta no puede ser anterior a la fecha desde",
          2
        );
        return;
      }
    }

    let json = {
      id: this.noteId,
      statusId: this.stateId == 0 ? null : this.stateId,
      creationUserId: this.applicantId,
      fromDate: this.dateSince,
      toDate: this.dateTo,
      providerId: this.providerId,
      analyticId: this.analyticId,
    };

    json.fromDate =
      this.dateSince == null || this.dateSince == undefined
        ? null
        : `${this.dateSince.getFullYear()}/${
            this.dateSince.getMonth() + 1
          }/${this.dateSince.getDate()}`;

    json.toDate =
      this.dateTo == null || this.dateTo == undefined
        ? null
        : `${this.dateTo.getFullYear()}/${
            this.dateTo.getMonth() + 1
          }/${this.dateTo.getDate()}`;

    this.messageService.showLoading();

    this.requestNoteService.getAll(json).subscribe((d) => {
      this.notes = [];
      this.notesInProcess = [];
      this.notesEnded = [];

      if (d.length > 0) {
        d.forEach((note) => {
          // ! Código muerto.
          //note.status = this.states[note.statusId].text;

          note.status = note.statusId;

          this.notesBackup.push(note);

          this.notesBackup = [...this.notesBackup];

          if (note.status == "Cerrada" || note.status == "Rechazada") {
            this.notesEnded.push(note);
            this.notesEnded = [...this.notesEnded];
          } else {
            this.notesInProcess.push(note);
            this.notesInProcess = [...this.notesInProcess];
          }
        });

        this.notes = this.notesInProcess;
      }

      if (json.statusId == 11 || json.statusId == 12) {
        this.tabTwo.nativeElement.click();
      } else {
        this.tabOne.nativeElement.click();
      }

      this.messageService.closeLoading();
    });

    this.collapse();
  }

  public refreshSearch(): void {
    this.stateId = null;
    this.applicantId = null;
    this.dateSince = null;
    this.dateTo = null;
    this.providerId = null;
    this.noteId = null;
  }

  public addOC(id: number): void {
    this.purchaseOrderService.setId(id);
    this.router.navigate([`providers/purchase-orders/nueva`]);
  }

  public changeTab(tab?: number): void {
    if (tab) {
      this.currentTab = tab;
    }
    if (this.currentTab == 2) {
      this.notes = this.notesEnded;
    }
    if (this.currentTab == 1) {
      this.notes = this.notesInProcess;
    }
    this.initGrid();
  }

  public getProviders(id: number): void {
    this.modalProvidersList = [];
    this.messageService.showLoading();
    this.requestNoteService.getProviders(id).subscribe((d) => {
      this.modalProvidersList = d;
      this.modalProvidersList.sort((a, b) => {
        const nameA = a.name.toUpperCase();
        const nameB = b.name.toUpperCase();
        return nameA < nameB ? -1 : 1;
      });
      this.messageService.closeLoading();
      // ! No existe el método show() dentro del componente 'Ng2ModalConfig' | app/components/modal/ng2modal-config
      // this.providersModal.show();
    });
  }

  public view(id: number): void {
    this.requestNoteService.setMode("View");
    this.router.navigate([`providers/notes/edit/${id}`]);
  }

  public edit(id: number) {
    this.requestNoteService.setMode("Edit");
    this.router.navigate([`providers/notes/edit/${id}`]);
  }

  private inicializar(): void {
    this.analyticService.getByCurrentUserRequestNote().subscribe((d) => {
      this.analiticas = d.data;
    });

    var json = {
      statusId: null,
      creationUserId: null,
      fromDate: null,
      toDate: null,
      providerId: null,
      noteId: null,
      analyticId: null,
    };

    setTimeout(() => {
      this.requestNoteService.getAll(json).subscribe((d) => {
        d.forEach((note: any) => {
          // ! Código muerto.
          // if(note.statusId <= 12) {
          //     note.status = this.states[note.statusId].text;
          // } else {
          //     note.status = note.statusId;
          // }

          note.status = note.statusId;

          this.notesBackup.push(note);

          this.notesBackup = [...this.notesBackup];

          if (note.status == "Cerrada" || note.status == "Rechazada") {
            this.notesEnded.push(note);
            this.notesEnded = [...this.notesEnded];
          } else {
            this.notesInProcess.push(note);
            this.notesInProcess = [...this.notesInProcess];
          }
        });

        this.notes = this.notesInProcess;

        this.notes.sort(function (a, b) {
          return b.id - a.id;
        });

        this.initGrid();

        setTimeout(() => {
          let el: HTMLElement = document.getElementsByClassName(
            "sorting_asc"
          )[0] as HTMLElement;
          el.click();
        }, 501);
      });
    }, 500);

    this.employeeService.getEveryone().subscribe((d) => {
      d.forEach((employee: any) => {
        if (employee.endDate == null && employee.isExternal == 0) {
          this.applicants.push(employee);
          this.applicants = [...this.applicants];
        }
      });
    });

    this.providerService.getAll().subscribe((d) => {
      d.data.forEach((provider) => {
        if (provider.active == true) {
          this.providers.push(provider);
          this.providers = [...this.providers];
        }
      });
    });

    this.collapse();
  }

  private initGrid(): void {
    var columns = [0, 1, 2, 3, 4, 5];

    var params = {
      selector: "#dataTable",
      columns: columns,
      title: "Notas",
      withExport: true,
    };

    this.dataTableService.destroy(params.selector);
    this.dataTableService.initialize(params);
    setTimeout(() => {
      let el: HTMLElement = document.getElementsByClassName(
        "sorting_asc"
      )[0] as HTMLElement;
      el.click();
    }, 501);
    this.messageService.closeLoading();
  }

  // ! Función no utilizada.
  // private formatDate(date: Date): void {
  //   let finalDate = {
  //     day: "",
  //     month: "",
  //     year: "",
  //   };
  // }
}
