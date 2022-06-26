import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { ProvidersService } from "app/services/admin/providers.service";
import { RequestNoteService } from "app/services/admin/request-note.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'notes',
    templateUrl: './notes.html'
})

export class NotesComponent implements OnInit{

    applicants = [];
    applicantId: number;
    states = [
        { id: 0, text: "Todos" },
        { id: 1, text: "Borrador" },
        { id: 2, text: "Pendiente Revisión Abastecimiento" },
        { id: 3, text: "Pendiente Aprobación Gerentes Analítica" },
        { id: 4, text: "Pendiente Aprobación Abastecimiento" },
        { id: 5, text: "Pendiente Aprobación DAF" },
        { id: 6, text: "Aprobada" },
        { id: 7, text: "Solicitada a Proveedor" },
        { id: 8, text: "Recibido Conforme" },
        { id: 9, text: "Factura Pendiente Aprobación Gerente" },
        { id: 10, text: "Pendiente Procesar GAF" },
        { id: 11, text: "Rechazada" },
        { id: 12, text: "Cerrada" },
    ];
    stateId: number;
    providers = [];
    providerId: number;
    dateSince;
    dateTo;
    notes = [];
    notesInProcess = [];
    notesEnded = [
        {
            id: 1,
            description: "Descripción",
            status: null,
            creationDate: "2022-6-24"
        }
    ];
    finalizedNotes = [];

    constructor(
        private employeeService: EmployeeService,
        private messageService: MessageService,
        private providerService: ProvidersService,
        private router: Router,
        private requestNoteService: RequestNoteService,
        private dataTableService: DataTableService
    ){}

    ngOnInit(): void {
        this.inicializar();
    }

    inicializar() {
        this.requestNoteService.getAll().subscribe(d=>{
            console.log(d);
            this.notes = d;
            this.notesInProcess = d;
        });
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
        this.initGrid();
        this.collapse();
    }

    view(id: number) {
        this.router.navigate([`providers/notes/edit/${id}`]);
    }

    edit(id: number) {
        this.router.navigate([`providers/notes/edit/${id}`]);
    }

    search() {
        if(this.dateTo != null && this.dateTo != undefined && this.dateSince != null && this.dateSince != undefined) {
            if(this.dateTo < this.dateSince) {
                this.messageService.showMessage("La fecha hasta no puede ser anterior a la fecha desde", 2);
                return;
            }
        }
        var json = {
            stateId: this.stateId,
            applicantId: this.applicantId,
            dateSince: this.dateSince,
            dateTo: this.dateTo,
            providerId: this.providerId,
        };
        console.log("Búsqueda");
        console.log(json)

    }

    refreshSearch() {
        this.stateId = null;
        this.applicantId = null;
        this.dateSince = null;
        this.dateTo = null;
        this.providerId = null;
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
        var columns = [0, 1, 2, 3, 4];
    
        var params = {
            selector: '#dataTable',
            columns: columns,
            title: 'Notas',
            withExport: true
        }
    
        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
    }

    changeTab(tab: number) {
        if(tab == 2) {
            this.notes = this.notesEnded;
        }
        if(tab == 1) {
            this.notes = this.notesInProcess;
        }
        this.initGrid();
    }
}