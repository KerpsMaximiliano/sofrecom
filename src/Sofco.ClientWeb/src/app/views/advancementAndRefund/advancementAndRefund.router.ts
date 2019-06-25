import { Routes, RouterModule } from "@angular/router";
import { AuthGuard } from "../../guards/auth.guard";
import { AdvancementDetailComponent } from "./advancement/detail/advancement-detail.componet";
import { AdvancementSearchComponent } from "./advancement/search/advancement-search.component";
import { RefundListComponent } from "./refund/list/refund-list.component";
import { RefundDetailComponent } from "./refund/detail/refund-detail.component";
import { ListPaymentPendingComponent } from "./common/list-payment-pending/list-payment-pending.component";
import { AdvancementRefundSettingComponent } from "./common/settings/setting.component";
import { CurrentAccountComponent } from "./common/current-account/current-account";
import { RefundDelegateComponent } from "./refund/delegate/refund-delegate";
import { SalaryAdvancementComponent } from "./common/salary-advancement/salary-advancement";

const ADVANCEMENT_AND_REFUND_ROUTER: Routes = [
    {
        path: "advancement",
        children: [
            { path: "search", component: AdvancementSearchComponent, canActivate: [AuthGuard], data: { module: "ADVAN", functionality: "QUERY" }  },
            { path: "paymentPending", component: ListPaymentPendingComponent, canActivate: [AuthGuard], data: { module: "ADVAN", functionality: "PAYMENT-PENDING-VIEW" } },
            { path: ":id", component: AdvancementDetailComponent, canActivate: [AuthGuard] }
        ]},
    {
        path: "refund",
        children: [
            { path: "search", component: RefundListComponent, canActivate: [AuthGuard], data: { module: "ADVAN", functionality: "REFUND-LIST-VIEW" }  },
            { path: ":id", component: RefundDetailComponent, canActivate: [AuthGuard] }
        ]
    },
    {
        path: "setting", component: AdvancementRefundSettingComponent, canActivate: [AuthGuard], data: { module: "ADVAN", functionality: "SETTING" }
    },
    {
        path: "delegate", component: RefundDelegateComponent, canActivate: [AuthGuard]
    },
    {
        path: "currentAccount", component: CurrentAccountComponent, canActivate: [AuthGuard], data: { module: "ADVAN", functionality: "CURRENT-ACCOUNT" }
    },
    {
        path: "salaryReturns", component: SalaryAdvancementComponent, canActivate: [AuthGuard], data: { module: "ADVAN", functionality: "SALARY-ADVANCEMENT" }
    }
]

export const AdvancementAndRefundRouter = RouterModule.forChild(ADVANCEMENT_AND_REFUND_ROUTER);
