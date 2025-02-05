import { CommonModule } from "@angular/common";
import { Ng2DatatablesModule } from "app/components/datatables/ng2-datatables.module";
import { RouterModule } from "@angular/router";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { ICheckModule } from "app/components/icheck/icheck.module";
import { Ng2ModalModule } from "app/components/modal/ng2modal.module";
import { TranslateModule } from "@ngx-translate/core";
import { ButtonsModule } from "app/components/buttons/buttons.module";
import { NgSelectModule } from "@ng-select/ng-select";
import { BsDatepickerModule } from "ngx-bootstrap";
import { NgModule } from "@angular/core";
import { UtilsService } from "app/services/common/utils.service";
import { FormsService } from "app/services/forms/forms.service";
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { RefundFormComponent } from "./refund-form.component";
import { AnalyticService } from "app/services/allocation-management/analytic.service";
import { AmountFormatModule } from "app/pipes/amount-format.module";
import { GenericOptionService } from "app/services/admin/generic-option.service";

@NgModule({
    imports: [
        CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule,
        TranslateModule, ButtonsModule, NgSelectModule, ReactiveFormsModule, BsDatepickerModule, AmountFormatModule
    ],
    declarations: [ 
        RefundFormComponent
    ],
    providers: [
        UtilsService, AdvancementService, FormsService, AnalyticService, GenericOptionService,
    ],
    exports: [
        RefundFormComponent
    ]
  })
  export class RefundFormModule { }
   