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
import { BsDatepickerModule } from "ngx-bootstrap";
import { AdvancementAndRefundRouter } from "./advancementAndRefund.router";
import { ReactiveFormsModule } from '@angular/forms';
import { AdvancementService } from "app/services/advancement-and-refund/advancement.service";
import { AdvancementDetailComponent } from "./advancement/detail/advancement-detail.componet";
import { AdvacementFormModule } from "./advancement/form/advancement-form.module";
import { WorkflowModule } from "app/components/workflow/workflow.module";
import { AdvancementListInProcessComponent } from "./advancement/list-in-process/list-in-process.component";
import { AdvancementHistoryComponent } from "./advancement/history/advancement-histories.component";
import { AdvancementSearchComponent } from "./advancement/search/advancement-search.component";
import { NgSelectModule } from "@ng-select/ng-select";
import { I18nService } from "app/services/common/i18n.service";
import { UserService } from "app/services/admin/user.service";
import { AdvancementListFinalizedComponent } from "./advancement/list-finalized/list-finalized.component";
import { RefundListComponent } from "./refund/list/refund-list.component";
import { RefundDetailComponent } from "./refund/detail/refund-detail.component";
import { RefundService } from "app/services/advancement-and-refund/refund.service";
import { RefundListFilterComponent } from "./refund/list/common/refund-list-filter.component";
import { RefundListGridComponent } from "./refund/list/common/refund-list-grid.component";
import { RefundFormModule } from "./refund/form/refund-form.module";
import { RefundHistoryComponent } from "./refund/history/refund-history.component";
import { ButtonsModule } from "app/components/buttons/buttons.module";
import { AmountFormatModule } from "app/pipes/amount-format.module";
import { ListPaymentPendingComponent } from "./common/list-payment-pending/list-payment-pending.component";
import { AdvancementRefundSettingComponent } from "./common/settings/setting.component";
import { AdvancementRefundSettingService } from "app/services/advancement-and-refund/setting.service";

@NgModule({
    declarations: [ AdvancementDetailComponent, AdvancementListInProcessComponent, AdvancementHistoryComponent,
                    AdvancementSearchComponent, AdvancementListFinalizedComponent, RefundDetailComponent,
                    RefundListComponent, RefundListFilterComponent, RefundListGridComponent, RefundHistoryComponent,
                    ListPaymentPendingComponent, AdvancementRefundSettingComponent
                  ],

    imports     : [ CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule, RefundFormModule,
                   TranslateModule, FileUploadModule, LayoutsModule, SpinnerModule, DatePickerModule, ButtonsModule, WorkflowModule,
                   AdvancementAndRefundRouter, ReactiveFormsModule, BsDatepickerModule, AdvacementFormModule, NgSelectModule, AmountFormatModule ],

    providers   : [ AdvancementService, UserService, I18nService, RefundService, AdvancementRefundSettingService ],

    exports     : []
})

export class AdvancementAndRefundModule {}
