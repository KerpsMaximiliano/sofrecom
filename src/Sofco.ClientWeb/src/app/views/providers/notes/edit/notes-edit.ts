import { AfterContentChecked, AfterContentInit, AfterViewChecked, AfterViewInit, ChangeDetectorRef, Component, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { Ng2ModalConfig } from "app/components/modal/ng2modal-config";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { MessageService } from "app/services/common/message.service";
import { environment } from "environments/environment";
import { forkJoin } from "rxjs";


@Component({
    selector: 'notes-edit',
    templateUrl: './notes-edit.html'
})

export class NotesEditComponent implements OnInit{

    states = [
        "", 
        "Borrador",
        "Pendiente Revisión Abastecimiento",
        "Pendiente Aprobación Gerentes Analítica",
        "Pendiente Aprobación Abastecimiento",
        "Pendiente Aprobación DAF",
        "Aprobada",
        "Solicitada a Proveedor",
        "Recibido Conforme",
        "Factura Pendiente Aprobación Gerente",
        "Pendiente Procesar GAF",
        "Rechazada",
        "Cerrada"
    ]

    estado: number;
    estadoSeleccionado: number;
    historyComments: string;
    histories = [
    ];
    currentNote;
    currentNoteStatusDescription;
    showEdit = false;
    lastStatusId: number = 0;
    closed: boolean = false;
    rejected: boolean = false;

    @ViewChild('commentsModal') commentsModal;

    public commentsModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "comments",
        "commentsModal",
        false,
        true,
        "ACTIONS.ACCEPT",
        "ACTIONS.close" 
    );

    constructor(
        private requestNoteService: RequestNoteService,
        private route: ActivatedRoute,
        private messageService: MessageService,
        private router: Router
    ){}

    ngOnInit(): void {
        forkJoin([this.requestNoteService.getById(this.route.snapshot.params['id']), this.requestNoteService.getHistories(this.route.snapshot.params['id'])]).subscribe(results => {
            if(results[0].messages.length >= 1) {
                if(results[0].messages[0].code == "notAllowed") {
                    this.router.navigate(['/providers/notes/no-access']);
                }
            } else {
                if(results[0].data.hasEditPermissions) {
                    
                } else if (results[0].data.hasReadPermissions){
                    this.requestNoteService.setMode("View")
                }
                this.currentNote = results[0].data;
                this.estado = this.currentNote.statusId;
                console.log(results[0].data)
                // if(this.estado > 10) {
                //     this.requestNoteService.setMode("View");
                //     this.currentNoteStatusDescription = (this.estado == 11) ? "Rechazada" : "Cerrada"
                //     this.estadoSeleccionado = results[1][results[1].length - 1].statusFromId
                // } else {
                //     this.estadoSeleccionado = this.estado;
                //     this.currentNoteStatusDescription = null;
                // };
                this.estadoSeleccionado = this.estado;
                this.currentNoteStatusDescription = null;
                if(this.requestNoteService.getMode() == "Edit" && results[0].hasEditPermissions == false) {
                    this.router.navigate(['/providers/notes/no-access']);
                };
                if(this.requestNoteService.getMode() == "View" && results[0].hasReadPermissions == false) {
                    this.router.navigate(['/providers/notes/no-access']);
                };
                results[1].forEach(history => {
                    let model = {
                        createdDate: history.createdDate,
                        userName: history.userName,
                        statusFrom: history.statusFromId,
                        statusTo: history.statusToId,
                        comment: history.comment
                    }
                    this.histories.push(model);
                    this.histories = [...this.histories]
                });
                if(this.currentNote.statusId == environment.NP_CERRADA) {
                    this.closed = true;
                    this.lastStatusId = this.histories[this.histories.length - 1].statusFrom;
                };
                if(this.currentNote.statusId == environment.NP_RECHAZADA) {
                    this.rejected = true;
                    this.lastStatusId = this.histories[this.histories.length - 1].statusFrom;
                }
                this.showEdit = true;
            }
        })
    }

    setEstado() {
        this.estadoSeleccionado = this.estado;
    }

    showComments(history) {
        this.historyComments = history.comment;
        this.commentsModal.show();
    }

    get currentEnviroment() {
        return environment;
    }
    
}