import { AfterViewInit, Component, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { forkJoin } from "rxjs";

// * Services
import { MessageService } from "app/services/common/message.service";
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

  /**
   * Configuración de valores en el inicio de la vista, 'filter'.
   */
  public selectedStates: number = 0;
  public businessName: string;
  public selectedAreas: [];
  public selectedCritical: number = 0;

  // dataTable.
  public data = [];

  constructor(
    private router: Router,
    private providersService: ProvidersService,
    private providersAreaService: ProvidersAreaService,
    private dataTableService: DataTableService,
    private messageService: MessageService
  ) {}

  ngAfterViewInit(): void {
    this.messageService.showLoading();

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
        this.messageService.closeLoading();
      },
      (error) => {
        const [providersError, areasError] = error;

        if (providersError)
          this.messageService.showMessage(
            "No se han podido cargar los proveedores.",
            1
          );

        if (areasError)
          this.messageService.showMessage(
            "No se han podido cargar los rubros.",
            1
          );

        this.messageService.closeLoading();
      }
    );
  }

  public view(id: number): void {
    this.providersService.setMode("View");
    this.router.navigate([`providers/providers/edit/${id}`]);
  }

  public edit(id: number): void {
    this.providersService.setMode("Edit");
    this.router.navigate([`providers/providers/edit/${id}`]);
  }

  public clear(): void {
    this.selectedStates = 0;
    this.businessName = "";
    this.selectedAreas = null;
    this.selectedCritical = 0;
  }

  public search(): void {
    this.messageService.showLoading();

    this.providersService
      .getByParams({
        statusId: this.selectedStates === 0 ? null : this.selectedStates,
        businessName: this.businessName,
        providersArea: this.selectedAreas,
      })
      .subscribe({
        next: (res: any) => {
          this.providers$ = Array.isArray(res.data)
            ? (res.data as IProviders[])
            : [res.data as IProviders];
        },
        error: (err: any) => {},
        complete: () => {
          this.setData();
          this.messageService.closeLoading();
        },
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

  private initGrid(): void {
    this.dataTableService.destroy("#dataTable");
    this.dataTableService.initialize({
      selector: "#dataTable",
      columns: [0, 1, 2, 3, 4, 5, 6],
      title: "Proveedores",
      withExport: true,
    });
  }
}
