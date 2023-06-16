import { Component, OnDestroy, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AutomaticHoursService } from "app/services/admin/automatic-hours.service";
import { MenuService } from "app/services/admin/menu.service";
import { SettingsService } from "app/services/admin/settings.service";
import { MessageService } from "app/services/common/message.service";

@Component({
    selector: 'app-automatic-hours',
    templateUrl: './automatic-hours.component.html',
    styleUrls: ['./automatic-hours.component.css']
})


export class AutomaticHoursComponent implements OnInit {
 
    form: FormGroup = new FormGroup({
        task: new FormControl(null, Validators.required)
    });

    tasks = [] as Array<any>;

    setting: any;

    constructor(
        private automaticHoursService: AutomaticHoursService,
        public menuService: MenuService,
        private settingsService: SettingsService,
        private messageService: MessageService
    ) {}

    ngOnInit(): void {
        this.settingsService.getAll().subscribe(d => {
            this.setting = d.body.data.find(setting => setting.id == 27);
            this.form.controls['task'].setValue(Number(this.setting ? this.setting.value : null));
        })
        this.automaticHoursService.getTasks().subscribe(d => {
            this.tasks = d;
        });
    }

    save() {
        if (this.form.invalid) {
            this.markFormGroupTouched(this.form);
            return;
        };
        let model = {
            id: 27,
            key: this.setting.key,
            category: this.setting.category,
            type: this.setting.type,
            value: Number(this.form.get('task').value)
        };
        this.settingsService.put(model).subscribe(d => {
            this.messageService.showMessage("Cambios guardados", 0);
        },
        err => {
            this.messageService.showMessage("Error al guardar cambios", 1);
        })
    }

    run() {
        if (this.form.invalid) {
            this.markFormGroupTouched(this.form);
            return;
        };
        this.automaticHoursService.runProcess().subscribe(d => {
            this.messageService.showMessage("Proceso de carga de horas ejecutado", 0);
        },
        err => {
            this.messageService.showMessage("Error al ejecutar el proceso", 1);
        })
    }

    markFormGroupTouched(formGroup: FormGroup) {
        (<any>Object).values(formGroup.controls).forEach(control => {
            control.markAsTouched();

            if (control.controls) {
                this.markFormGroupTouched(control);
            }
        });
    };
}