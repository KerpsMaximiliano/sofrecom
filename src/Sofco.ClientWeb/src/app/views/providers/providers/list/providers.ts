import { MessageService } from 'app/services/common/message.service';
import { Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { IVAConditions } from "app/models/enums/IVAConditions";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { DataTableService } from "app/services/common/datatable.service";
import { forkJoin } from "rxjs";

/**
 * Interfaz para representar las propiedades de un Rubro
 */
interface ProviderArea {
  active?: boolean;
  critical?: boolean;
  description?: string;
  endDate?: string | null;
  startDate?: string | null;
  id?: number;
}

/**
 * Tipo de dato para estandarizar la estructura de
 * las opciones de los ng-select
 */
type SelectOption = { id: number, text: string };

@Component({
    selector: 'providers',
    templateUrl: './providers.html',
    styleUrls: [
      './providers.scss',
    ]
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
    states: SelectOption[] = [
        { id: 0, text: "Todos" },
        { id: 1, text: "Activo" },
        { id: 2, text: "Inactivo" },
    ];
    critical: SelectOption[] = [
      { id: 0, text: "Todos" },
      { id: 1, text: "Sí" },
      { id: 2, text: "No" },
    ];
    stateId: number = 0;
    businessName: string;
    areas = [];
    criticalSearch: [] | SelectOption['id'] = []; // No es de opción múltiple, toma el valor literlal del id
    areaId = [];
    data = [];
    dataBackup = [];
    backupProviderAreas = [];

    constructor(
        private router: Router,
        private providersService: ProvidersService,
        private providersArea: ProvidersAreaService,
        private dataTableService: DataTableService,
        private messageService: MessageService
    ) {}

    ngOnInit(): void {
        this.providersArea.getAll().subscribe(d => {
            d.data.forEach(providerArea => {
                if(providerArea.active) {
                    this.areas.push(providerArea);
                    this.areas = [...this.areas]
                }
            });
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
        this.router.navigate([`providers/providers/detail/${id}`]);
    }

    edit(id) {
        this.router.navigate([`providers/providers/edit/${id}`])
    }

    refreshSearch() {
        this.stateId = null;
        this.businessName = null;
        this.areaId = null;

    }

    search() {        
    this.messageService.showLoading();
        this.providersService.getByParams({statusId: this.stateId == 0 ? null : this.stateId, businessName: this.businessName, providersArea: this.areaId}).subscribe(d => {
            this.data = [];
            d.data.forEach(this.setData.bind(this));
            this.initGrid();
            this.messageService.closeLoading();
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

        // Filtrado por crítico en el front
        const passCritical =
          Array.isArray(this.criticalSearch) || this.criticalSearch === this.critical[0].id ||  // Caso sin filtro o "Todos"
          (this.criticalSearch === this.critical[1].id && someCritical) ||                      // Caso filtro = "Sí"
          (this.criticalSearch === this.critical[2].id && someNotCritical);                     // Caso filtro = "No"

        if(passCritical) {
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
