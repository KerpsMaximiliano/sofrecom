import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { NgSelectModule } from "@ng-select/ng-select";
import { TranslateModule } from "@ngx-translate/core";
import { IboxtoolsModule } from "app/components/common/iboxtools/iboxtools.module";
import { LayoutsModule } from "app/components/common/layouts/layouts.module";
import { Ng2DatatablesModule } from "app/components/datatables/ng2-datatables.module";
import { ICheckModule } from "app/components/icheck/icheck.module";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { Select2Module } from "app/components/select2/select2";
import { SpinnerModule } from "app/components/spinner/spinner.module";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { FileUploadModule } from "ng2-file-upload";
import { ButtonsModule } from "ngx-bootstrap";
import { NotesAddComponent } from "./notes/add/notes-add";
import { NotesEditComponent } from "./notes/edit/notes-edit";
import { NotesComponent } from "./notes/list/notes";
import { ProvidersRouter } from "./providers.router";
import { ProvidersAddComponent } from "./providers/add/providers-add";
import { ProvidersEditComponent } from "./providers/edit/providers-edit";
import { ProvidersComponent } from "./providers/list/providers";


@NgModule({
    declarations: [
        ProvidersComponent,
        ProvidersAddComponent,
        ProvidersEditComponent,
        NotesComponent,
        NotesAddComponent,
        NotesEditComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        LayoutsModule,
        FormsModule,
        ICheckModule,
        TranslateModule,
        Select2Module,
        SpinnerModule,
        IboxtoolsModule,
        ButtonsModule,
        NgSelectModule,
        ReactiveFormsModule,
        ProvidersRouter,
        Ng2DatatablesModule,
        Ng2ModalModule,
        FileUploadModule,
    ],
    providers: [
        EmployeeService
    ],
    exports: []
})

export class ProvidersModule {};