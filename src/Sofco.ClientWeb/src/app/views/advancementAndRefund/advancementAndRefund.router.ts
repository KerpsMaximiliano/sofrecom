import { Routes, RouterModule } from "@angular/router";
import { AuthGuard } from "../../guards/auth.guard";
import { AdvancementDetailComponent } from "./advancement/detail/advancement-detail.componet";

const ADVANCEMENT_AND_REFUND_ROUTER: Routes = [
    {
        path: "advancement",
        children: [
            { path: ":id", component: AdvancementDetailComponent, canActivate: [AuthGuard], data: { module: "ADVAN", functionality: "ADD" } } 
        ]
    }
]

export const AdvancementAndRefundRouter = RouterModule.forChild(ADVANCEMENT_AND_REFUND_ROUTER);