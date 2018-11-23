import { Routes, RouterModule } from "@angular/router";
import { AuthGuard } from "../../guards/auth.guard";
import { AdvancementDetailComponent } from "./advancement/detail/advancement-detail.componet";
import { AdvancementListInProcessComponent } from "./advancement/list-in-process/list-in-process.component";

const ADVANCEMENT_AND_REFUND_ROUTER: Routes = [
    {
        path: "advancement",
        children: [
            { path: "search", component: AdvancementListInProcessComponent, canActivate: [AuthGuard], data: { module: "ADVAN", functionality: "QUERY" }  },
            { path: ":id", component: AdvancementDetailComponent, canActivate: [AuthGuard] }
        ]
    }
]

export const AdvancementAndRefundRouter = RouterModule.forChild(ADVANCEMENT_AND_REFUND_ROUTER);