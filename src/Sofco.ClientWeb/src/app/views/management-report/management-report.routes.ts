import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "app/guards/auth.guard";
import { ManagementReportDetailComponent } from "./detail/mr-detail";
import { ManagementReportDetailStaffComponent } from "./staff/detail/detail-staff";
import { ManagementReportOnLeave } from "app/guards/managementReportOnLeave.guard";

const MANAGEMENT_REPORT_ROUTER: Routes = [
    { path: ":customerId/service/:serviceId/detail",  
        component: ManagementReportDetailComponent, 
        canActivate: [AuthGuard], 
        canDeactivate: [ManagementReportOnLeave], 
        data: { module: "MANRE", functionality: "VIEW-DETAIL" } },

    { path: ":id",  
        component: ManagementReportDetailStaffComponent, 
        canActivate: [AuthGuard], 
        canDeactivate: [ManagementReportOnLeave], 
        data: { module: "MANRE", functionality: "VIEW-DETAIL" } },
];

export const ManagementReportRouter = RouterModule.forChild(MANAGEMENT_REPORT_ROUTER);