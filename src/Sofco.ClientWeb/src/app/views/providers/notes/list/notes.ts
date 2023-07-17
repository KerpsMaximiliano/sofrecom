import { AfterViewInit, Component, ElementRef, OnInit, ViewChild } from "@angular/core";
import { Router } from "@angular/router";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { Ng2ModalComponent } from "app/components/modal/ng2modal.component";
import { ProvidersService } from "app/services/admin/providers.service";
import { PurchaseOrderService } from "app/services/admin/purchase-order.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";
import { environment } from "environments/environment";

export interface Note {
  creationDate: string;
  creationUserDescription: string;
  creationUserId: number;
  creationUserName: string;
  description: string;
  hasEditPermissions: boolean;
  hasReadPermissions: boolean;
  id: number;
  status: number;
  statusDescription: string;
  statusId: number;
}

export enum AnaliticState {
  PENDING_APPROVAL =  "Pendiente de Aprobación",
  APPROVED =  "Aprobada",
}

export interface AnaliticManager {
  name: string;
  status: AnaliticState;
}

@Component({
    selector: 'notes',
    templateUrl: './notes.html',
    styleUrls: [
      './analitic-managers.scss',
    ],
})

export class NotesComponent implements OnInit, AfterViewInit{

    modalProvidersList = [];
    applicants = [];
    applicantId: number;
    currentTab: number = 1;
    states2 = environment.REQUEST_NOTE_STATES;
    stateId: number;
    providers = [];
    providerId: number;
    dateSince;
    dateTo;
    noteId;
    notes = [];
    notesBackup = [];
    notesInProcess = [];
    notesEnded = [];

    analiticManagers: { [key: string]: Set<string> } = {
      approved: new Set<string>(),
      pending_approval: new Set<string>(),
    }

    @ViewChild('tabOne') tabOne: ElementRef;
    @ViewChild('tabTwo') tabTwo: ElementRef;
    @ViewChild('providersModal') providersModal;
    public providersModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "Proveedores designados",
        "rejectModal",
        false,
        true,
        "",
        "Cerrar"
    );

    analiticStates = AnaliticState;

    @ViewChild('approvedManagersModal') approvedManagersModal;
    public approvedManagersModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
      "PMs Aprobadores",
      "managersModal",
      false,
      true,
      "",
      "Cerrar"
  );

    constructor(
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private providerService: ProvidersService,
        private router: Router,
        private requestNoteService: RequestNoteService,
        private dataTableService: DataTableService,
        private purchaseOrderService: PurchaseOrderService
    ){}

    ngOnInit(): void {
        //this.inicializar();
    }

    ngAfterViewInit(): void {
        this.inicializar();
    }

    inicializar() {

        var json = {
            statusId: null,
            creationUserId: null,
            fromDate: null,
            toDate: null,
            providerId: null,
            noteId: null
        };
        setTimeout(() => {
            this.requestNoteService.getAll(json).subscribe(d=>{
                d.forEach(note => {
                    // if(note.statusId <= 12) {
                    //     note.status = this.states[note.statusId].text;
                    // } else {
                    //     note.status = note.statusId;
                    // }
                    note.status = note.statusId;
                    this.notesBackup.push(note);
                    this.notesBackup = [...this.notesBackup];
                    if(note.status == "Cerrada" || note.status == "Rechazada") {
                        this.notesEnded.push(note);
                        this.notesEnded = [...this.notesEnded];
                    } else {
                        this.notesInProcess.push(note);
                        this.notesInProcess = [...this.notesInProcess];
                    }
                })
                this.notes = this.notesInProcess;
                this.notes.sort(function(a, b) {
                    return b.id - a.id;
                });
                console.log(this.notes)
                this.initGrid();
                setTimeout(() => {
                    let el: HTMLElement = document.getElementsByClassName('sorting_asc')[0] as HTMLElement;
                    el.click()
                }, 501);
            });
        }, 500);

        this.employeeService.getEveryone().subscribe(d => {
            d.forEach(employee => {
                if(employee.endDate == null && employee.isExternal == 0) {
                    this.applicants.push(employee);
                    this.applicants = [...this.applicants]
                }
            });
        });
        this.providerService.getAll().subscribe(d => {
            d.data.forEach(provider => {
                if(provider.active == true) {
                    this.providers.push(provider);
                    this.providers = [...this.providers]
                }
            })
        });
        this.collapse();
    }

    view(id: number) {
        this.requestNoteService.setMode("View");
        this.router.navigate([`providers/notes/edit/${id}`]);
    }

    edit(id: number) {
        this.requestNoteService.setMode("Edit");
        this.router.navigate([`providers/notes/edit/${id}`]);
    }

    addOC(id: number) {
        this.purchaseOrderService.setId(id);
        this.router.navigate([`providers/purchase-orders/nueva`]);
    }

    getProviders(id: number) {
        this.modalProvidersList = [];
        this.messageService.showLoading();
        this.requestNoteService.getProviders(id).subscribe(d => {
            console.log(d);
            this.modalProvidersList = d;
            this.modalProvidersList.sort((a, b) => {
                const nameA = a.name.toUpperCase();
                const nameB = b.name.toUpperCase();
                return nameA < nameB ? -1 : 1
              });
            this.messageService.closeLoading();
            this.providersModal.show();
        });
    }

    search() {
        if(this.dateTo != null && this.dateTo != undefined && this.dateSince != null && this.dateSince != undefined) {
            if(this.dateTo < this.dateSince) {
                this.messageService.showMessage("La fecha hasta no puede ser anterior a la fecha desde", 2);
                return;
            }
        }
        let json = {
            id: this.noteId,
            statusId: (this.stateId == 0) ? null : this.stateId,
            creationUserId: this.applicantId,
            fromDate: this.dateSince,
            toDate: this.dateTo,
            providerId: this.providerId,
        };
        json.fromDate = (this.dateSince == null || this.dateSince == undefined) ? null : `${this.dateSince.getFullYear()}/${this.dateSince.getMonth() + 1}/${this.dateSince.getDate()}`;
        json.toDate = (this.dateTo == null || this.dateTo == undefined) ? null : `${this.dateTo.getFullYear()}/${this.dateTo.getMonth() + 1}/${this.dateTo.getDate()}`;
        this.requestNoteService.getAll(json).subscribe(d => {
            this.notes = [];
            this.notesInProcess = [];
            this.notesEnded = [];
            if(d.length > 0) {
                d.forEach(note => {
                    //note.status = this.states[note.statusId].text;
                    note.status = note.statusId;
                    this.notesBackup.push(note);
                    this.notesBackup = [...this.notesBackup];
                    if(note.status == "Cerrada" || note.status == "Rechazada") {
                        this.notesEnded.push(note);
                        this.notesEnded = [...this.notesEnded];
                    } else {
                        this.notesInProcess.push(note);
                        this.notesInProcess = [...this.notesInProcess];
                    }
                })
                this.notes = this.notesInProcess;
            };
            if(json.statusId == 11 || json.statusId == 12) {
                this.tabTwo.nativeElement.click();
            } else {
                this.tabOne.nativeElement.click();
            };
        });
    }

    refreshSearch() {
        this.stateId = null;
        this.applicantId = null;
        this.dateSince = null;
        this.dateTo = null;
        this.providerId = null;
        this.noteId = null;
    }

    collapse() {
        if ($("#collapseOne").hasClass('in')) {
            $("#collapseOne").removeClass('in');
        }
        else {
            $("#collapseOne").addClass('in');
        }

        this.changeIcon();
    }

    changeIcon() {
        if ($("#collapseOne").hasClass('in')) {
            $("#search-icon").toggleClass('fa-caret-down').toggleClass('fa-caret-up');
        }
        else {
            $("#search-icon").toggleClass('fa-caret-up').toggleClass('fa-caret-down');
        }
    }

    initGrid() {
        var columns = [0, 1, 2, 3, 4, 5];

        var params = {
            selector: '#dataTable',
            columns: columns,
            title: 'Notas',
            withExport: true
        }

        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
    }

    changeTab(tab?: number) {
        if(tab) {
            this.currentTab = tab;
        };
        if(this.currentTab == 2) {
            this.notes = this.notesEnded;
        };
        if(this.currentTab == 1) {
            this.notes = this.notesInProcess;
        };
        this.initGrid();
    }

    formatDate(date: Date) {
        let finalDate = {
            day: '',
            month: '',
            year: ''
        };
    }

    get currentEnviroment() {
        return environment;
    }

    getApprovedManagers(noteId: Note["id"]) {
      for(let managerSet in this.analiticManagers) {
        this.analiticManagers[managerSet].clear();
      }
      this.messageService.showLoading();
      this.approvedManagersModal.show();
      this.requestNoteService.getApprovedManagers<AnaliticManager>(noteId).subscribe(
        (managers) => {
          this.messageService.closeLoading();
          for(let manager of managers) {
            if(manager.status == AnaliticState.APPROVED) {
              this.analiticManagers.approved.add(manager.name);
            } else if(manager.status == AnaliticState.PENDING_APPROVAL) {
              this.analiticManagers.pending_approval.add(manager.name);
            }
          }
        }
      );
    }
}
