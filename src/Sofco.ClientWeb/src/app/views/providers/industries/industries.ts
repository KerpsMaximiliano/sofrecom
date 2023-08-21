import { AfterViewInit, Component } from "@angular/core";
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
export class IndustriesComponent implements AfterViewInit {
  public data = []; // dataTable.

  public description: string = ""; // INPUT: Descripción.
  public critical: number = 0; // SELECT: Critico.
  public rnAmmountReq: boolean = false; // CHECKBOX: Requiere monto en notas de pedido.

  constructor(
    private _industries: ProvidersAreaService,
    private _table: DataTableService,
    private _message: MessageService,
    private router: Router
  ) {}

  ngAfterViewInit(): void {
    this._message.showLoading();
    if (this._industries.getMode() && this._industries.getIndustry()) {
      this.data.push(this._industries.getIndustry());
      this._industries.setMode(false);
      this._message.closeLoading();
    } else {
      this._industries.getAll().subscribe({
        next: (res: any) => this.setDataTable(res.data),
        error: () => this._message.closeLoading(), // ! Sin manejo de errores.
        complete: () => this._message.closeLoading(),
      });
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

  public search(): void {
    this._message.showLoading();
    this._industries.getAll().subscribe({
      next: (res: any) => this.setDataTable(res.data),
      error: () => this._message.closeLoading(), // ! Sin manejo de errores.
      complete: () => this._message.closeLoading(),
    });
  }

  public clear(): void {
    this.critical = 0;
    this.description = "";
    this.rnAmmountReq = false;
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
      next: () => {
        this.updateTable(industry.id);
        this._message.showMessage(
          `El rubro fue ${
            industry.active ? "habilitado. " : "deshabilitado. "
          }`,
          0
        );
      },
      error: () => this._message.closeLoading(), // ! Sin manejo de errores.
      complete: () => {},
    });
  }

  private updateTable(id: number): void {
    let index = this.data.findIndex((industry) => industry.id === id);
    if (index === -1) return;
    this._industries.get(id).subscribe({
      next: (res: any) => {
        this.data[index] = res.data;
      },
      error: () => this._message.closeLoading(),
      complete: () => this._message.closeLoading(),
    });
  }

  private setDataTable(data: any): void {
    this.data = [];
    data.forEach((element: any) => {
      this.data.push(element);
    });
    this.data = [...this.data];
    if (this.data.length > 1) this.initGrid();
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
  }
}
