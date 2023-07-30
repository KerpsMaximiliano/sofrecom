import { AfterViewInit, Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { forkJoin } from "rxjs";

// * Services.
import { DataTableService } from "app/services/common/datatable.service";
import { ProvidersService } from "app/services/admin/providers.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";

// * Interfaces.
import {
  IProviders,
  IProvidersArea,
  IProvidersAreaProvider,
} from "app/models/providers/providers.interface";

@Component({
  selector: "providers",
  templateUrl: "./providers.html",
})
export class ProvidersComponent implements AfterViewInit {
  private ivaConditions: string[] = [
    "Resp. Inscripto",
    "Resp. No Inscripto",
    "Monotributo",
    "Exento/No Resp.",
  ];
  private grossIncome: string[] = [
    "Inscripto en régimen local General",
    "Inscripto en régimen local simplificado",
    "Inscripto Convenio Multilateral",
    "Exento",
    "No aplica",
  ];

  // Lista de provedores.
  private providers$: IProviders[] = [];

  // Lista de areas criticas y no criticas.
  private criticalAreas: IProvidersArea[] = [];
  private notCriticalAreas: IProvidersArea[] = [];

  // Lista de areas (select | Rubro).
  public areas$: IProvidersArea[] = [];

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

  public selectedCritical: number = 0;

  constructor(
    private router: Router,
    private providersService: ProvidersService,
    private providersAreaService: ProvidersAreaService,
    private dataTableService: DataTableService
  ) {}

  // ngOnInit(): void {
  // this.providersArea.getAll().subscribe((d) => {
  //   d.data.forEach((providerArea) => {
  //     if (providerArea.active) {
  //       this.areas.push(providerArea);
  //       this.areas = [...this.areas];
  //     }
  //   });
  // });
  // forkJoin([
  //   this.providersService.getAll(),
  //   this.providersArea.getAll(),
  // ]).subscribe((res) => {
  //   this.backupProviderAreas = res[1].data;
  //   res[0].data.forEach((provider) => {
  //     let areas = [];
  //     provider.providersAreaProviders.forEach((p) => {
  //       areas.push(
  //         res[1].data.find((prov) => prov.id == p.providerAreaId).description
  //       );
  //     });
  //     let model = {
  //       id: provider.id,
  //       name: provider.name,
  //       providerArea: null,
  //       providerAreaId: null,
  //       CUIT: provider.cuit,
  //       ingresosBrutos:
  //         provider.ingresosBrutos != null
  //           ? this.grossIncome[provider.ingresosBrutos - 1]
  //           : "",
  //       condicionIVA:
  //         provider.condicionIVA != null
  //           ? this.ivaConditions[provider.condicionIVA - 1]
  //           : "",
  //       active: provider.active,
  //     };
  //     if (areas.length > 0) {
  //       model.providerArea = areas;
  //       //model.providerAreaId = area.id
  //     }
  //     this.data.push(model);
  //     this.data = [...this.data];
  //   });
  //   this.dataBackup = this.data;
  //   this.initGrid();
  // });
  // }

  ngAfterViewInit(): void {
    const providersData$ = this.providersService.getAll();
    const areasData$ = this.providersAreaService.getAll();

    forkJoin([providersData$, areasData$]).subscribe(
      ([providersResponse, areasResponse]) => {
        this.providers$ = Array.isArray(providersResponse.data)
          ? (providersResponse.data as IProviders[])
          : [providersResponse.data as IProviders];

        areasResponse.data.forEach((area: IProvidersArea) => {
          if (area.active === true) {
            this.areas$.push(area);
            area.critical
              ? this.criticalAreas.push(area)
              : this.notCriticalAreas.push(area);
          }
        });

        this.areas$ = [...this.areas$];
        this.setData();
      }
    );
  }

  view(id) {
    this.providersService.setMode("View");
    this.router.navigate([`providers/providers/edit/${id}`]);
  }

  edit(id) {
    this.providersService.setMode("Edit");
    this.router.navigate([`providers/providers/edit/${id}`]);
  }

  refreshSearch() {
    this.stateId = null;
    this.businessName = null;
    this.areaId = null;
    this.selectedCritical = 0;
  }

  search() {
    this.providersService
      .getByParams({
        statusId: this.stateId == 0 ? null : this.stateId,
        businessName: this.businessName,
        providersArea: this.areaId,
      })
      .subscribe((d) => {
        this.data = [];
        d.data.forEach((prov) => {
          let areas = [];
          prov.providersAreaProviders.forEach((p) => {
            areas.push(
              this.backupProviderAreas.find(
                (provider) => provider.id == p.providerAreaId
              ).description
            );
          });
          let model = {
            id: prov.id,
            name: prov.name,
            providerArea: null,
            providerAreaId: null,
            CUIT: prov.cuit,
            ingresosBrutos:
              prov.ingresosBrutos != null
                ? this.grossIncome[prov.ingresosBrutos - 1]
                : "",
            condicionIVA:
              prov.condicionIVA != null
                ? this.ivaConditions[prov.condicionIVA - 1]
                : "",
            active: prov.active,
          };
          if (areas.length > 0) {
            model.providerArea = areas;
            //model.providerAreaId = area.id
          }
          this.data.push(model);
          this.data = [...this.data];
        });
        this.initGrid();
      });
  }

  private setData(): void {
    this.data = [];
    this.providers$.forEach((provider: IProviders) => {
      this.reviewProvider(provider);
    });
    this.data = [...this.data];
    this.initGrid();
  }

  private reviewProvider(provider: IProviders): void {
    switch (this.selectedCritical) {
      case 1:
        this.reviewArea(provider, this.criticalAreas);
        break;
      case 2:
        this.reviewArea(provider, this.notCriticalAreas);
        break;
      default:
        this.reviewArea(provider, this.areas$);
        break;
    }
  }

  private reviewArea(provider: IProviders, areas: IProvidersArea[]): void {
    let areasProviders = [];
    provider.providersAreaProviders.forEach(
      (element: IProvidersAreaProvider) => {
        let area: IProvidersArea = areas.find(
          (a) => a.id == element.providerAreaId
        );
        if (area) {
          areasProviders.push(area.description);
        }
      }
    );

    if (!areasProviders.length) return;

    this.data.push({
      id: provider.id,
      name: provider.name,
      providerArea: areasProviders.length ? areasProviders : null,
      cuit: provider.cuit,
      ingresosBrutos: provider.ingresosBrutos
        ? this.grossIncome[provider.ingresosBrutos - 1]
        : "",
      condicionIVA: provider.condicionIVA
        ? this.ivaConditions[provider.condicionIVA - 1]
        : "",
    });
  }

  initGrid() {
    var columns = [0, 1, 2, 3, 4, 5, 6];

    var params = {
      selector: "#dataTable",
      columns: columns,
      title: "Proveedores",
      withExport: true,
    };

    this.dataTableService.destroy(params.selector);
    this.dataTableService.initialize(params);
  }
}
