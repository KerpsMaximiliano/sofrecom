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
  styleUrls: ["./providers.scss"],
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
    this.router.navigate([`providers/providers/detail/${id}`]);
  }

  public edit(id: number): void {
    this.providersService.setMode("Edit");
    this.router.navigate([`providers/providers/edit/${id}`]);
  }

  public down(id: number): void {
    let provider: IProviders = this.providers$.find(
      (provider: IProviders) => provider.id == id
    );

    if (provider) this.setProvider(provider);
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

  public changeIcon(): void {
    $("#collapseOne").hasClass("in")
      ? $("#search-icon")
          .toggleClass("fa-caret-down")
          .toggleClass("fa-caret-up")
      : $("#search-icon")
          .toggleClass("fa-caret-up")
          .toggleClass("fa-caret-down");
  }

  public collapse(): void {
    $("#collapseOne").hasClass("in")
      ? $("#collapseOne").removeClass("in")
      : $("#collapseOne").addClass("in");

    this.changeIcon();
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
    this.providers$.forEach((provider: IProviders) =>
      this.reviewProvider(provider)
    );
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
    if (this.hasCriticalArea(provider)) return;
    let areas = [];
    provider.providersAreaProviders.forEach(
      (element: IProvidersAreaProvider) => {
        let area: IProvidersArea = this.notCriticalAreas.find(
          (a) => a.id == element.providerAreaId
        );
        if (area) areas.push(area.description);
      }
    );
    if (areas.length) this.setDataTable(provider, areas);
  }

  /**
   * Determina si el proveedor tiene algún área critica, si la tiene
   * busca todas las áreas que coinciden con las del proveedor y
   * añade al proveedor a la lista de proveedores.
   * @param provider Proveedor.
   * @returns Retorna si no existe un área coincidente con la del proveedor.
   */
  private criticalArea(provider: IProviders): void {
    if (!this.hasCriticalArea(provider)) return;
    let areas = [];
    provider.providersAreaProviders.forEach(
      (element: IProvidersAreaProvider) => {
        let area: IProvidersArea = this.areas$.find(
          (a) => a.id == element.providerAreaId
        );
        if (area) {
          areas.push(area.description);
        }
      }
    );
    if (areas.length) this.setDataTable(provider, areas);
  }

  /**
   * Busca todas las coincidencias respecto a las áreas de un proveedor y
   * añade al proveedor a la lista de proveedores.
   * @param provider Proveedor.
   * @returns Retorna si no existe un área coincidente con la del proveedor.
   */
  private allArea(provider: IProviders): void {
    let areas: string[] = [];
    provider.providersAreaProviders.forEach(
      (element: IProvidersAreaProvider) => {
        let area: IProvidersArea = this.areas$.find(
          (a) => a.id == element.providerAreaId
        );
        if (area) areas.push(area.description);
      }
    );
    if (areas.length) this.setDataTable(provider, areas);
  }

  /**
   * Comprueba si un proveedor tiene al menos un área crítica.
   * @param provider Proveedor.
   * @returns Boolean: true = Tiene áreas críticas || false = No tiene áreas críticas.
   */
  private hasCriticalArea(provider: IProviders): boolean {
    for (const element of provider.providersAreaProviders) {
      const area: IProvidersArea = this.criticalAreas.find(
        (a) => a.id === element.providerAreaId
      );
      if (area) return true; // Si se encuentra un área crítica, el proveedor tiene áreas críticas
    }
    return false; // Si no se encontraron áreas críticas, el proveedor no tiene áreas críticas
  }

  /**
   * Recibé un proveedor y su/s área/s y lo añade a la lista de proveedores.
   * @param provider Proveedor.
   * @param areas Lista de áreas que coinciden con la/s del proveedor.
   */
  private setDataTable(provider: IProviders, areas: string[]): void {
    let ingresosBrutos: string = "";
    if (provider.ingresosBrutos)
      ingresosBrutos = this.grossIncome[provider.ingresosBrutos - 1];

    let condicionIVA: string = "";
    if (provider.condicionIVA)
      condicionIVA = this.ivaConditions[provider.condicionIVA - 1];

    this.data.push({
      id: provider.id,
      name: provider.name,
      active: provider.active,
      providerArea: areas.length ? areas : null,
      cuit: provider.cuit,
      ingresosBrutos: ingresosBrutos,
      condicionIVA: condicionIVA,
    });
  }

  /**
   * 1. Destruye el servicio (si existiese) de la grilla (tabla).
   * 2. Construye el servicio para la grilla (tabla).
   */
  private initGrid(): void {
    let config = {
      selector: "#dataTable",
      columns: [0, 1, 2, 3, 4, 5, 6],
      title: "Proveedores",
      withExport: true,
    };

    this.dataTableService.destroy(config.selector);
    this.dataTableService.initialize(config);
  }

  /**
   * Modifica el estado de un proveedor y llama a la función updateDate().
   * @param provider Proveedor que se desea modificar.
   */
  private setProvider(provider: IProviders): void {
    this.messageService.showLoading();

    provider.active
      ? (provider.endDate = null)
      : (provider.endDate = new Date());
    provider.active = !provider.active;

    this.providersService.edit(provider.id, provider).subscribe({
      next: (res: any) => {
        this.messageService.showMessage(
          `El proveedor fue ${
            !provider.active ? "deshabilitado" : "habilitado"
          }`,
          0
        );
        this.updateDate(provider.id);
      },
      error: (err: any) => {
        this.messageService.showMessage(
          `No se ha podido ${
            provider.active ? "deshabilitar" : "habilitar"
          } el proveedor. `,
          0
        );
      },
      complete: () => {
        this.messageService.closeLoading();
      },
    });
  }

  /**
   * Cambia el estado de un proveedor y actualiza la vista.
   * @param id ID del proveedor al que se le cambia el estado.
   */
  updateDate(id: number): void {
    let provider = this.data.findIndex((provider) => provider.id === id);
    if (provider === -1) return;
    this.data[provider].active = !this.data[provider].active;
  }
}
