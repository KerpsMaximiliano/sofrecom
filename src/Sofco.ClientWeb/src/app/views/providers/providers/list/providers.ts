import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { IVAConditions } from "app/models/enums/IVAConditions";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { DataTableService } from "app/services/common/datatable.service";
import { forkJoin } from "rxjs";

@Component({
    selector: 'providers',
    templateUrl: './providers.html'
})

export class ProvidersComponent implements OnInit{


    ivaConditions = [
        'Resp. Inscripto',
        'Resp. No Inscripto',
        'Monotributo',
        'Exento/No Resp.'
    ];
    grossIncome = [
        'Inscripto en régimen local General',
        'Inscripto en régimen local simplificado',
        'Inscripto Convenio Multilateral',
        'Exento',
        'No aplica'
    ]
    states = [
        { id: 0, text: "Todos" },
        { id: 1, text: "Activo" },
        { id: 2, text: "Inactivo" },
    ];
    stateId: number = 1;
    businessName: string;
    areas = [];
    areaId: number;
    data = [];
    dataBackup = [];

    constructor(
        private router: Router,
        private providersService: ProvidersService,
        private providersArea: ProvidersAreaService,
        private dataTableService: DataTableService
    ) {}

    ngOnInit(): void {
        this.providersArea.getAll().subscribe(d => {
            d.data.forEach(providerArea => {
                if(providerArea.active) {
                    this.areas.push(providerArea);
                    this.areas = [...this.areas]
                }
            })
        });
        forkJoin([
            this.providersService.getAll(),
            this.providersArea.getAll()
        ]).subscribe(res => {
            res[0].data.forEach(provider => {
                let area = res[1].data.find(provArea => provArea.id == provider.providerAreaId);
                let model = {
                    id: provider.id,
                    name: provider.name,
                    providerArea: null,
                    providerAreaId: null,
                    CUIT: provider.cuit,
                    ingresosBrutos: this.grossIncome[provider.ingresosBrutos + 1],
                    condicionIVA: this.ivaConditions[provider.condicionIVA + 1],
                    active: provider.active
                }
                if(area != undefined) {
                    model.providerArea = area.description,
                    model.providerAreaId = area.id
                }
                this.data.push(model);
                this.data = [...this.data]
            });
            this.dataBackup = this.data;
            this.initGrid();
        });
    }

    view(id) {
        this.providersService.setMode("View");
        this.router.navigate([`providers/providers/edit/${id}`]);
    }

    edit(id) {
        this.providersService.setMode("Edit");
        this.router.navigate([`providers/providers/edit/${id}`])
    }

    refreshSearch() {
        this.stateId = null;
        this.businessName = null;
        this.areaId = null
    }

    search() {
        let searchData = [];
        if (this.stateId == 0 || this.stateId == null) {
            searchData = this.dataBackup;
        } else {
            let estado;
            if (this.stateId == 1) {
                estado = true;
            }
            if (this.stateId == 2) {
                estado = false
            }
            this.dataBackup.forEach(entry => {
                if(entry.active == estado) {
                    searchData.push(entry)
                }
            })
        };
        if(this.areaId != null) {
            let search = [];
            searchData.forEach(entry => {
                if(entry.providerAreaId == this.areaId) {
                    search.push(entry)
                }
            });
            searchData = search;
        };
        if(this.businessName != null && this.businessName.length > 0) {
            let search = [];
            searchData.forEach(entry => {
                let result = entry.name.toLowerCase().includes(this.businessName.toLocaleLowerCase());
                if(result) {
                    search.push(entry);
                }
            });
            searchData = search;
        }
        this.data = searchData;
        this.initGrid();
    }

    initGrid() {
        var columns = [0, 1, 2, 3, 4, 5, 6];
    
        var params = {
            selector: '#dataTable',
            columns: columns,
            title: 'Proveedores',
            withExport: true
        }
    
        this.dataTableService.destroy(params.selector);
        this.dataTableService.initialize(params);
    }
}