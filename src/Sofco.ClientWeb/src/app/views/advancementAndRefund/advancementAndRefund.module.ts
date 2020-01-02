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
import { RefundsRelatedComponent } from "./advancement/refunds-related/refunds-related";
import { CurrentAccountService } from "app/services/advancement-and-refund/current-account.sevice";
import { CurrentAccountComponent } from "./common/current-account/current-account";
import { UserApproverService } from "app/services/allocation-management/user-approver.service";
import { PCheckModule } from "app/components/pcheck/pcheck.module";
import { PaymentPendingService } from "app/services/advancement-and-refund/paymentPending.service";
import { SalaryAdvancementService } from "app/services/advancement-and-refund/salary-advancement";
import { SalaryAdvancementComponent } from "./common/salary-advancement/salary-advancement";

@NgModule({
    declarations: [ AdvancementDetailComponent, AdvancementHistoryComponent, SalaryAdvancementComponent,
                    AdvancementSearchComponent, AdvancementListFinalizedComponent, RefundDetailComponent,
                    RefundListComponent, RefundListFilterComponent, RefundListGridComponent, RefundHistoryComponent,
                    ListPaymentPendingComponent, AdvancementRefundSettingComponent, RefundsRelatedComponent, CurrentAccountComponent
                  ],

    imports     : [ CommonModule, Ng2DatatablesModule, RouterModule, FormsModule, ICheckModule, Ng2ModalModule, RefundFormModule, PCheckModule,
                   TranslateModule, FileUploadModule, LayoutsModule, SpinnerModule, DatePickerModule, ButtonsModule, WorkflowModule,
                   AdvancementAndRefundRouter, ReactiveFormsModule, BsDatepickerModule, AdvacementFormModule, NgSelectModule, AmountFormatModule ],

    providers   : [ AdvancementService, UserService, RefundService, PaymentPendingService,  SalaryAdvancementService,
                    AdvancementRefundSettingService, CurrentAccountService, UserApproverService ],

    exports     : []
})

export class AdvancementAndRefundModule {}
