import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { TranslateModule } from "@ngx-translate/core";
import { FileUploadModule } from "ng2-file-upload";
import { Ng2DatatablesModule } from "../../components/datatables/ng2-datatables.module";
import { RouterModule } from "@angular/router";
import { LayoutsModule } from "../../components/common/layouts/layouts.module";
import { SpinnerModule } from "../../components/spinner/spinner.module";
import { FormsModule } from "@angular/forms";
import { ICheckModule } from "../../components/icheck/icheck.module";
import { DatePickerModule } from "../../components/date-picker/date-picker.module";
import { Ng2ModalModule } from "../../components/modal/ng2modal.module";
import { ButtonsModule, BsDatepickerModule } from "ngx-bootstrap";
import { AdvancementAndRefundRouter } from "./advancementAndRefund.router";
import { ReactiveFormsModule } from '@angular/forms';
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { AdvancementDetailComponent } from "./advancement/detail/advancement-detail.componet";
import { AdvacementFormModule } from "./advancement/form/advancement-form.module";
import { WorkflowModule } from "app/components/workflow/workflow.module";
import { AdvancementListInProcessComponent } from "./advancement/list-in-process/list-in-process.component";
 
@NgModule({
    declarations: [ AdvancementDetailComponent, AdvancementListInProcessComponent ],

    imports     : [ CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
                   TranslateModule, FileUploadModule, LayoutsModule, SpinnerModule, DatePickerModule, ButtonsModule, WorkflowModule,
                   AdvancementAndRefundRouter, ReactiveFormsModule, BsDatepickerModule, AdvacementFormModule ],

    providers   : [ AdvancementService ],

    exports     : []
})

export class AdvancementAndRefundModule {} 