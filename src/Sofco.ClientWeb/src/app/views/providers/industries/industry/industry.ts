import { AfterContentInit, AfterViewInit, Component } from "@angular/core";
import {
  FormBuilder,
  FormControl,
  FormGroup,
  Validators,
} from "@angular/forms";

// * Services.
import { MessageService } from "app/services/common/message.service";
import { ProvidersAreaService } from "app/services/admin/providersArea.service";
import { ActivatedRoute, Router } from "@angular/router";

@Component({
  selector: "industry",
  templateUrl: "./industry.html",
  styleUrls: ["./industry.scss"],
})
export class IndustryComponent implements AfterContentInit {
  private industry: any;
  private param: string = this.route.snapshot.routeConfig.path;
  private id: number = this.route.snapshot.params.id;

  public mode: string;
  public form: FormGroup;
  public edit: boolean = false;
  public active: any;
  public startDate: any;
  public endDate: any;

  constructor(
    private _industries: ProvidersAreaService,
    private _message: MessageService,
    private formBuilder: FormBuilder,
    private route: ActivatedRoute,
    private router: Router
  ) {
    this.configForm();
  }

  ngAfterContentInit(): void {
    if (this.param) this.setMode();
    if (this.param.includes("add")) return;
    if (this._industries.getIndustry()) {
      this.industry = this._industries.getIndustry();
      this.setForm(this.industry);
    } else {
      if (this.id) this.getIndustry(this.id);
    }
  }

  public confirm(): void {
    if (!this.form.valid) {
      this.markFormGroupTouched(this.form);
      this._message.showMessage("Por favor, verifica el formulario.", 1);
      return;
    }

    let form: any = {
      description: this.form.get("description").value,
      critical: this.form.get("critical").value,
      rnAmmountReq: this.form.get("rnAmmountReq").value,
      active: this.industry.active,
      startDate: this.industry.startDate,
      endDate: this.industry.endDate,
    };

    if (this.param.includes("add")) this.addIndustry(form);
    if (this.param.includes("edit")) this.setIndustry(this.id, form);
  }

  private setMode(): void {
    if (this.param.includes("view")) {
      this.mode = "Ver";
      this.edit = false;
      this.form.disable();
      return;
    }
    if (this.param.includes("add")) this.mode = "Agregar";
    if (this.param.includes("edit")) this.mode = "Editar";
    this.edit = true;
  }

  private markFormGroupTouched(form: FormGroup): void {
    (<any>Object).values(form.controls).forEach((control: any) => {
      control.markAsTouched();
      if (control.controls) this.markFormGroupTouched(control);
    });
  }

  private addIndustry(form: any): void {
    this._message.showLoading();
    this._industries.post(form).subscribe({
      next: (res: any) => {
        this._industries.setIndustry(res.data);
        this._industries.setMode(true);
        this._message.showMessage("El rubro fue creado exitosamente.", 0);
        this.router.navigate([`providers/industries`]);
      },
      error: () => this._message.closeLoading(), // ! Sin manejo de errores.
      complete: () => this._message.closeLoading(),
    });
  }

  private setIndustry(id: number, form: any): void {
    this._message.showLoading();

    this._industries.edit(id, form).subscribe({
      next: (res: any) => {
        let industry: any = res.data;
        industry.id = this.id;
        this._industries.setIndustry(industry);
        this._industries.setMode(true);
        this._message.showMessage("El rubro fue editado exitosamente.", 0);
        this.router.navigate([`providers/industries`]);
      },
      error: () => this._message.closeLoading(), // ! Sin manejo de errores.
      complete: () => this._message.closeLoading(),
    });
  }

  private getIndustry(id: number): void {
    this._message.showLoading();
    this._industries.get(id).subscribe({
      next: (res: any) => {
        this.industry = res.data;
        this._industries.setIndustry(res.data);
        this.setForm(res.data);
      },
      error: () => this._message.closeLoading(), // ! Sin manejo de errores.
      complete: () => this._message.closeLoading(),
    });
  }

  private configForm(): void {
    this.form = this.formBuilder.group({
      description: new FormControl(
        null,
        Validators.compose([Validators.required, Validators.maxLength(50)])
      ),
      critical: new FormControl(null, Validators.required),
      rnAmmountReq: new FormControl(null, Validators.required),
    });
  }

  private setForm(industry: any): void {
    this.form.get("description").setValue(industry.description);
    this.startDate = industry.startDate;
    this.endDate = industry.endDate;
    this.active = industry.active;

    if (this.edit) {
      this.form.get("critical").setValue(industry.critical);
      this.form.get("rnAmmountReq").setValue(industry.rnAmmountReq);
      return;
    }

    this.form.get("critical").setValue(industry.critical ? "Si" : "No");
    this.form.get("rnAmmountReq").setValue(industry.rnAmmountReq ? "Si" : "No");
  }

  public getErrorMessage(control: FormControl): string {
    if (control.errors && control.errors.required) {
      return `Este campo es obligatorio.`;
    } else if (control.errors.minlength) {
      return `Debe tener al menos ${control.errors.minlength.requiredLength} caracteres.`;
    } else if (control.errors.maxlength) {
      return `No puede tener más de ${control.errors.maxlength.requiredLength} caracteres.`;
    }
    return "";
  }
}
