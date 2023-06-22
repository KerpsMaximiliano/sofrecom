import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { IVAConditions } from "app/models/enums/IVAConditions";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { DataTableService } from "app/services/common/datatable.service";
import { forkJoin } from "rxjs";

/** Posibles estados de la propiedad crítico */
enum CriticalProvider {
  YES = 'Sí',
  NO = 'No',
  BOTH = 'Todo',
}

interface ProviderArea {
  active?: boolean;
  critical?: boolean;
  description?: string;
  endDate?: string | null;
  startDate?: string | null;
  id?: number;
}

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
    ];
    states = [
        { id: 0, text: "Todos" },
        { id: 1, text: "Activo" },
        { id: 2, text: "Inactivo" },
    ];
    critical = [
      CriticalProvider.YES,
      CriticalProvider.NO,
      CriticalProvider.BOTH,
    ];
    stateId: number = 0;
    businessName: string;
    areas = [];
    criticalSearch = [];
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
            res[0].data.forEach(this.setData.bind(this));
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
        this.areaId = null;

    }

    search() {
        // Filtrado por crítico en el "back" (sólo busca proveedores con rubros críticos)
        const areas = this.criticalSearch.length === 1 && this.criticalSearch[0] !== CriticalProvider.BOTH ?
          this.areaId.filter((area)=> area.critical === (this.criticalSearch[0] == CriticalProvider.YES ? true : false)) :
          this.areaId;

        this.providersService.getByParams({statusId: this.stateId == 0 ? null : this.stateId, businessName: this.businessName, providersArea: areas}).subscribe(d => {
            this.data = [];
            d.data.forEach(this.setData.bind(this));
            this.initGrid();
        })
    }

    setData(provider) {
        let someCritical = false,
          someNotCritical = false;
        const areas = [];
        provider.providersAreaProviders.forEach(p => {
            const area: ProviderArea = this.backupProviderAreas.find(prov => prov.id == p.providerAreaId);
            someCritical = someCritical || (area.critical === true);
            someNotCritical = someNotCritical || (area.critical === false);
            areas.push(area.description);
        });
        let model = {
            id: provider.id,
            name: provider.name,
            providerArea: null,
            providerAreaId: null,
            CUIT: provider.cuit,
            ingresosBrutos: provider.ingresosBrutos != null ? this.grossIncome[provider.ingresosBrutos - 1] : '',
            condicionIVA: provider.condicionIVA != null ? this.ivaConditions[provider.condicionIVA - 1] : '',
            active: provider.active,
            critical: null,
        }
        if(areas.length > 0) {
            model.providerArea = areas
            //model.providerAreaId = area.id
        }
        if(someCritical && someNotCritical) {
          model.critical = CriticalProvider.BOTH;
        } else if (someCritical || someNotCritical) {
          model.critical = someCritical ? CriticalProvider.YES : CriticalProvider.NO;
        }

        // Filtrado por crítico en el front
        if(!this.criticalSearch.length || this.criticalSearch.includes(model.critical)) {
          this.data.push(model);
          this.data = [...this.data]
        }
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
