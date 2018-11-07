import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { TranslateModule } from "@ngx-translate/core";
import { NgSelectModule } from "@ng-select/ng-select";
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
import { AdvancementAddComponent } from "app/views/advancementAndRefund/advancement/add/advancement-add.component";
import { AdvancementFormComponent } from "app/views/advancementAndRefund/advancement/form/advancement-form.component";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { UtilsService } from "app/services/common/utils.service";
import { UserService } from "app/services/admin/user.service";

@NgModule({
    declarations: [AdvancementAddComponent, AdvancementFormComponent],

    imports     : [CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
                    TranslateModule, FileUploadModule, LayoutsModule, SpinnerModule, DatePickerModule, ButtonsModule, 
                    NgSelectModule, AdvancementAndRefundRouter, ReactiveFormsModule, BsDatepickerModule],

    providers   : [AdvancementService, AnalyticService, UtilsService, UserService],

    exports     : []
})

export class AdvancementAndRefundModule {} 