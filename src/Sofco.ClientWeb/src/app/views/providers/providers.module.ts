import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { NgSelectModule } from "@ng-select/ng-select";
import { TranslateModule } from "@ngx-translate/core";
import { IboxtoolsModule } from "app/components/common/iboxtools/iboxtools.module";
import { LayoutsModule } from "app/components/common/layouts/layouts.module";
import { Ng2DatatablesModule } from "app/components/datatables/ng2-datatables.module";
import { DatePickerModule } from "app/components/date-picker/date-picker.module";
import { ICheckModule } from "app/components/icheck/icheck.module";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { Select2Module } from "app/components/select2/select2";
import { SpinnerModule } from "app/components/spinner/spinner.module";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { SalaryAdvancementService } from "app/services/advancement-and-refund/salary-advancement";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { EmployeeService } from "app/services/allocation-management/employee.service";
import { FileService } from "app/services/common/file.service";
import { FileUploadModule } from "ng2-file-upload";
import { BsDatepickerModule, ButtonsModule, ModalModule } from "ngx-bootstrap";
import { NotesAddComponent } from "./notes/add/notes-add";
import { NotesApproved } from "./notes/edit/approved/notes-approved";
import { NotesDraftComponent } from "./notes/edit/draft/notes-draft";
import { NotesEditComponent } from "./notes/edit/notes-edit";
import { NotesPendingApprovalManagementAnalytic } from "./notes/edit/pending-approval-management-analytic/notes-pending-approval-management-analytic";
import { NotesPendingDAFApproval } from "./notes/edit/pending-daf-approval/notes-pending-daf-approval";
import { NotesPendingGAFProcessing } from "./notes/edit/pending-gaf-processing/notes-pending-gaf-processing";
import { NotesPendingManagementBillApproval } from "./notes/edit/pending-management-bill-approval/notes-pending-management-bill-approval";
import { NotesPendingSupplyApproval } from "./notes/edit/pending-supply-approval/notes-pending-supply-approval";
import { NotesPendingSupplyRevision } from "./notes/edit/pending-supply-revision/notes-pending-supply-revision";
import { NotesReceivedConformable } from "./notes/edit/received-conformable/notes-received-conformable";
import { NotesRequestedProvider } from "./notes/edit/requested-provider/notes-requested-provider";
import { NotesComponent } from "./notes/list/notes";
import { NotesNoAccessComponent } from "./notes/no-access/notes-no-access";
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
        NotesEditComponent,
        NotesDraftComponent,
        NotesPendingSupplyRevision,
        NotesPendingApprovalManagementAnalytic,
        NotesPendingSupplyApproval,
        NotesPendingDAFApproval,
        NotesApproved,
        NotesRequestedProvider,
        NotesReceivedConformable,
        NotesPendingManagementBillApproval,
        NotesPendingGAFProcessing,
        NotesNoAccessComponent
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
        DatePickerModule,
        BsDatepickerModule,
        ModalModule
    ],
    providers: [
        EmployeeService,
        AnalyticService,
        RefundService,
        SalaryAdvancementService,
        FileService
    ],
    exports: []
})

export class ProvidersModule {};