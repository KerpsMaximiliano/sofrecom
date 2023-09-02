import { Component, OnInit } from "@angular/core";
import { DatePipe } from "@angular/common";
import { Router } from "@angular/router";

// * Services.
import { MessageService } from "app/services/common/message.service";
import { DataTableService } from "app/services/common/datatable.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";

@Component({
  selector: "industries",
  templateUrl: "./industries.html",
  styleUrls: ["./industries.scss"],
  providers: [DatePipe],
})
export class IndustriesComponent implements OnInit {
  public inputDescription: string = ""; // INPUT: Descripción.
  // 0 = Todos | 1 = Si | 2 = No
  public selectCritical: number = 0; // SELECT: Critico.
  // 0 = Todos | 1 = Si | 2 = No
  public selectRnAmmountReq: number = 0; // SELECT: Requiere monto en notas de pedido.

  public data = []; // dataTable.

  constructor(
    private _industries: ProvidersAreaService,
    private _table: DataTableService,
    private _message: MessageService,
    private router: Router
  ) {}

  ngOnInit(): void {
    if (this._industries.getMode() && this._industries.getIndustry()) {
      this.data = [];
      this.data.push(this._industries.getIndustry());
      this._industries.setMode(false);
      this._message.closeLoading();
    } else {
      this.search();
    }
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

  public search(option?: boolean): void {
    this._message.showLoading();
    this._industries.getAll().subscribe({
      next: (res: any) => this.setDataTable(res.data, option),
      error: () => this._message.closeLoading(), // ! Sin manejo de errores.
    });
  }

  public clear(): void {
    this.selectCritical = 0;
    this.selectRnAmmountReq = 0;
    this.inputDescription = "";
  }

  public mode(option: number, industry?: any): void {
    switch (option) {
      case 0:
        this.router.navigate([`providers/industries/industry/add`]);
        break;
      case 1:
        this._industries.setIndustry(industry);
        this.router.navigate([
          `providers/industries/industry/view`,
          industry.id,
        ]);
        break;
      case 2:
        this._industries.setIndustry(industry);
        this.router.navigate([
          `providers/industries/industry/edit`,
          industry.id,
        ]);
        break;
      default:
        break;
    }
  }

  public change(industry: any): void {
    this._message.showLoading();
    this._industries.action(industry, !industry.active).subscribe({
      next: (res: any) => {
        this.updateTable(industry.id);
        this._message.showMessage(
          `El rubro fue ${
            industry.active ? "habilitado. " : "deshabilitado. "
          }`,
          0
        );
      },
      error: () => this._message.closeLoading(), // ! Sin manejo de errores.
    });
  }

  private updateTable(id: number): void {
    let index: number = this.data.findIndex((industry) => industry.id === id);
    if (index === -1) return;
    this._industries.get(id).subscribe({
      next: (res: any) => {
        this.data[index] = res.data;
        this.setDataTable(this.data);
      },
      error: () => this._message.closeLoading(),
    });
  }

  private setDataTable(data: any, option?: boolean): void {
    this.data = [];
    data.forEach((element: any) => {
      option ? this.handleFilter(element) : this.data.push(element);
    });
    this.data = [...this.data];

    this.initGrid();
  }

  private handleFilter(element: any): void {
    // 0: todos.
    // 1: true.
    // 2: false.

    // critico: true/false && rnAmmountReq: true/false.
    if (this.selectCritical !== 0 && this.selectRnAmmountReq !== 0) {
      if (this.handleSelect(element, 1) && this.handleSelect(element, 2)) {
        this.filterDescription(element);
        return;
      }
    } else {
      // critico: TODOS && rnAmmountReq: TODOS.
      if (this.selectCritical === 0 && this.selectRnAmmountReq === 0) {
        this.filterDescription(element);
        return;
      }
    }

    // critico: TODOS && rnAmmountReq: true/false.
    if (this.selectCritical === 0 && this.selectRnAmmountReq !== 0) {
      if (this.handleSelect(element, 2)) this.filterDescription(element);
    }

    // critico: true/false && rnAmmountReq: TODOS.
    if (this.selectCritical !== 0 && this.selectRnAmmountReq === 0) {
      if (this.handleSelect(element, 1)) this.filterDescription(element);
    }
  }

  /**
   * Almacena el valor de la selección 'Critico' o 'rnAmmountReq' según la opción
   * recibida.
   *  1. Critico.
   *  2. RnAmmountReq.
   * Según la selección, almacena el valor de la propiedad del elemento en 'prop'.
   * @param element Rubro a filtrar.
   * @param option Selección con la que debe comparar.
   * @returns Boolean: si existe coincidencia entre la selección y el valor de element.
   */
  private handleSelect(element: any, option: number): boolean {
    let select: number = 0;
    let prop: boolean;

    if (option === 1) {
      select = this.selectCritical;
      prop = element.critical;
    }

    if (option === 2) {
      select = this.selectRnAmmountReq;
      prop = element.rnAmmountReq;
    }

    switch (select) {
      case 1:
        if (prop === true) return true;
        break;
      case 2:
        if (prop === false) return true;
        break;
      default:
        return false;
    }
  }

  private filterDescription(element: any): void {
    if (element.description.toLowerCase().includes(this.inputDescription)) {
      this.data.push(element);
    }
  }

  private initGrid(): void {
    let config = {
      selector: "#dataTable",
      columns: [0, 1, 2, 3, 4, 5, 6],
      title: "Rubros",
      withExport: true,
    };

    this._table.destroy(config.selector);
    this._table.initialize(config);

    this._message.closeLoading();
  }
}
