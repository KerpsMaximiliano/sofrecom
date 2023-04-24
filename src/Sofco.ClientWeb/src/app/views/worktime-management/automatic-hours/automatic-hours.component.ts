import { Component, OnDestroy, OnInit } from "@angular/core";
import { FormControl, FormGroup, Validators } from "@angular/forms";
import { AutomaticHoursService } from "app/services/admin/automatic-hours.service";
import { MenuService } from "app/services/admin/menu.service";

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

    constructor(
        private automaticHoursService: AutomaticHoursService,
        public menuService: MenuService,
    ) {}

    ngOnInit(): void {
        this.automaticHoursService.getTasks().subscribe(d => {
            console.log(d);
            this.tasks = d;
        });
    }

    save() {
        if (this.form.invalid) {
            this.markFormGroupTouched(this.form);
            return;
        }
        
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