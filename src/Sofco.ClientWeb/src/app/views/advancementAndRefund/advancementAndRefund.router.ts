import { Routes, RouterModule } from "@angular/router";
import { AuthGuard } from "../../guards/auth.guard";
import { AdvancementDetailComponent } from "./advancement/detail/advancement-detail.componet";
import { AdvancementSearchComponent } from "./advancement/search/advancement-search.component";
import { RefundDetailComponent } from "./refund/detail/refund-detail.component";

const ADVANCEMENT_AND_REFUND_ROUTER: Routes = [
    {
        path: "advancement",
        children: [
            { path: "search", component: AdvancementSearchComponent, canActivate: [AuthGuard], data: { module: "ADVAN", functionality: "QUERY" }  },
            { path: ":id", component: AdvancementDetailComponent, canActivate: [AuthGuard] }
        ]
    },
    {
        path: "refund",
        children: [
            { path: ":id", component: RefundDetailComponent, canActivate: [AuthGuard] }
        ]
    }
]

export const AdvancementAndRefundRouter = RouterModule.forChild(ADVANCEMENT_AND_REFUND_ROUTER);