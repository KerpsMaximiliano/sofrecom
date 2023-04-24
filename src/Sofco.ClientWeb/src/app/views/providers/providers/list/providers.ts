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
    stateId: number = 0;
    businessName: string;
    areas = [];
    areaId = [];
    data = [];
    dataBackup = [];
    backupProviderAreas = [];

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
            this.backupProviderAreas = res[1].data;
            res[0].data.forEach(provider => {
                let areas = [];
                provider.providersAreaProviders.forEach(p => {
                    areas.push(res[1].data.find(prov => prov.id == p.providerAreaId).description)
                });
                let model = {
                    id: provider.id,
                    name: provider.name,
                    providerArea: null,
                    providerAreaId: null,
                    CUIT: provider.cuit,
                    ingresosBrutos: provider.ingresosBrutos == null ? this.grossIncome[provider.ingresosBrutos - 1] : '',
                    condicionIVA: provider.condicionIVA == null ? this.ivaConditions[provider.condicionIVA - 1] : '',
                    active: provider.active
                }
                if(areas.length > 0) {
                    model.providerArea = areas
                    //model.providerAreaId = area.id
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
        this.providersService.getByParams({statusId: this.stateId == 0 ? null : this.stateId, businessName: this.businessName, providersArea: this.areaId}).subscribe(d => {
            this.data = [];
            d.data.forEach(prov => {
                let areas = [];
                prov.providersAreaProviders.forEach(p => {
                    areas.push(this.backupProviderAreas.find(provider => provider.id == p.providerAreaId).description)
                });
                let model = {
                    id: prov.id,
                    name: prov.name,
                    providerArea: null,
                    providerAreaId: null,
                    CUIT: prov.cuit,
                    ingresosBrutos: this.grossIncome[prov.ingresosBrutos + 1],
                    condicionIVA: this.ivaConditions[prov.condicionIVA + 1],
                    active: prov.active
                };
                if(areas.length > 0) {
                    model.providerArea = areas
                    //model.providerAreaId = area.id
                }
                this.data.push(model);
                this.data = [...this.data]
            });
            this.initGrid();
        })
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