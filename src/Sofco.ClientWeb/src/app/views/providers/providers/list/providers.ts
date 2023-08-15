import { AfterViewInit, Component } from "@angular/core";
import { Router } from "@angular/router";
import { forkJoin } from "rxjs";

// * Services.
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
  ) { }

  /**
   * Realiza dos peticiones:
   * ? providers$: Todos los proveedores.
   * ? areas$: Todas las áreas.
   *    > criticalAreas: Todas las áreas criticas.
   *    > notCriticalAreas: Todas las áreas no criticas.
   */
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
        // ! Actualiza el valor de 'areas$' para reflejar sus valores en el select 'Rubro'.
        this.areas$ = [...this.areas$];
        this.setData();
        this.messageService.closeLoading();
      },
      (error: any) => {
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

  /**
   * Limpia el filtro.
   */
  public clear(): void {
    this.selectedStates = 0;
    this.businessName = "";
    this.selectedAreas = null;
    this.selectedCritical = 0;
  }

  /**
   * Realiza busqueda de provedores según el 'Estado', 'Razón Social' y 'Rubro'.
   * Posterior a la busqueda (exitosa) filtra los provedores según si son o no 'Críticos'.
   */
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
        error: (err: any) => {
          // ! Sin manejo de errores.
          this.messageService.closeLoading();
        },
        complete: () => {
          this.setData();
          this.messageService.closeLoading();
        },
      });

  }

  /**
   * Recorre la lista de provedores, invoca a la función 'reviewProvider(provider)',
   * actualiza 'data' y genera la grilla (tabla).
   */
  private setData(): void {
    this.data = [];
    this.providers$.forEach((provider: IProviders) => {
      this.reviewProvider(provider);
    });
    this.data = [...this.data];
    this.initGrid();
  }

  /**
   * Recibé un proveedor y determina según el valor de 'selectedCritical',
   * la función de filtrado.
   * @param provider Proveedor.
   */
  private reviewProvider(provider: IProviders): void {
    switch (this.selectedCritical) {
      case 1:
        this.criticalArea(provider);
        break;
      case 2:
        this.notCriticalArea(provider);
        break;
      default:
        this.allArea(provider);
        break;
    }
  }

  /**
   * Determina si el proveedor tiene algún área critica, si no la tiene,
   * añade al proveedor a la lista de proveedores.
   * @param provider Proveedor.
   * @returns Retorna si no existe un área coincidente con la del proveedor.
   */
  private notCriticalArea(provider: IProviders): void {
    let areasProviders = [];

    provider.providersAreaProviders.forEach(
      (element: IProvidersAreaProvider) => {
        let area: IProvidersArea = this.criticalAreas.find(
          (a) => a.id == element.providerAreaId
        );
        if (!area) {
          let areaNoCritical: IProvidersArea = this.notCriticalAreas.find(
            (a) => a.id == element.providerAreaId
          )
          if (areaNoCritical) areasProviders.push(areaNoCritical.description);
        }
      }
    );
    if (!areasProviders.length) return;
    this.setDataTable(provider, areasProviders);
  }

  /**
   * Determina si el proveedor tiene algún área critica, si la tiene
   * busca todas las áreas que coinciden con las del proveedor y
   * añade al proveedor a la lista de proveedores.
   * @param provider Proveedor.
   * @returns Retorna si no existe un área coincidente con la del proveedor.
   */
  private criticalArea(provider: IProviders): void {
    let areasProviders = [];
    provider.providersAreaProviders.forEach(
      (element: IProvidersAreaProvider) => {
        let area: IProvidersArea = this.criticalAreas.find(
          (a) => a.id == element.providerAreaId
        );
        if (area) {
          let areaCritical: IProvidersArea = this.areas$.find(
            (a) => a.id == element.providerAreaId
          )
          if (areaCritical) areasProviders.push(areaCritical.description);
        }
      }
    );
    if (!areasProviders.length) return;
    this.setDataTable(provider, areasProviders);
  }

  /**
   * Busca todas las coincidencias respecto a las áreas de un proveedor y
   * añade al proveedor a la lista de proveedores.
   * @param provider Proveedor.
   * @returns Retorna si no existe un área coincidente con la del proveedor.
   */
  private allArea(provider: IProviders): void {
    let areasProviders: string[] = [];
    provider.providersAreaProviders.forEach(
      (element: IProvidersAreaProvider) => {
        let area: IProvidersArea = this.areas$.find(
          (a) => a.id == element.providerAreaId
        )
        if (area) {
          areasProviders.push(area.description);
        }
      }
    );
    if (!areasProviders.length) return;
    this.setDataTable(provider, areasProviders);
  }

  /**
   * Recibé un proveedor y su/s área/s y lo añade a la lista de proveedores.
   * @param provider Proveedor.
   * @param areas Lista de áreas que coinciden con la/s del proveedor.
   */
  private setDataTable(provider: IProviders, areas: string[]): void {
    this.data.push({
      id: provider.id,
      name: provider.name,
      providerArea: areas.length ? areas : null,
      cuit: provider.cuit,
      ingresosBrutos: provider.ingresosBrutos
        ? this.grossIncome[provider.ingresosBrutos - 1]
        : "",
      condicionIVA: provider.condicionIVA
        ? this.ivaConditions[provider.condicionIVA - 1]
        : "",
    });
  }

  /**
   * 1. Destruye el servicio (si existiese) de la grilla (tabla).
   * 2. Construye el servicio para la grilla (tabla).
   */
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
