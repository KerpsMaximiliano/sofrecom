import { Routes, RouterModule } from "@angular/router";
import { AuthGuard } from "../../guards/auth.guard";
import { AdvancementAddComponent } from "app/views/advancementAndRefund/advancement/add/advancement-add.component";

const ADVANCEMENT_AND_REFUND_ROUTER: Routes = [
    {
        path: "advancement",
        children: [
            { path: "add", component: AdvancementAddComponent, canActivate: [AuthGuard] } 
        ]
    }
]

export const AdvancementAndRefundRouter = RouterModule.forChild(ADVANCEMENT_AND_REFUND_ROUTER);