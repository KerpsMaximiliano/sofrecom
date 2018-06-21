import { Routes, RouterModule } from "@angular/router";
import { AllocationReportComponent } from "app/views/allocation-management/allocation/report/allocation-report.component";
import { AuthGuard } from "app/guards/auth.guard";
import { ResourceSearchComponent } from "app/views/allocation-management/resources/search/resource-search.component";
import { ResourceDetailComponent } from "app/views/allocation-management/resources/detail/resource-detail.component";
import { AddAllocationByResourceComponent } from "app/views/allocation-management/allocation/add-by-resource/add-by-resource.component";
import { WorkTimeApprovalDelegateComponent } from "app/views/allocation-management/worktime/worktime-approval-delegate/worktime-approval-delegate.component";

const ALLOCATION_ROUTER: Routes = [
    { path: "allocationsReport", component: AllocationReportComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "PMORP" } },
    {
      path: "resources",
      children: [
        { path: "", component: ResourceSearchComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "LSTRE" } },
        { path: ":id", component: ResourceDetailComponent, canActivate: [AuthGuard], data: { fromRrhh: true, module: "PROFI", functionality: "VWPRO" } },
        { path: ":id/allocations", component: AddAllocationByResourceComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "ADRES" } },
      ]
    },

    {
      path: 'workTimeApproval',
      children: [
        { path: "delegate", component: WorkTimeApprovalDelegateComponent, canActivate: [AuthGuard], data: { fromProfile: false, module: "ALLOC", functionality: "TAPDE" } }
      ]
    },
];

export const AllocationRouter = RouterModule.forChild(ALLOCATION_ROUTER);