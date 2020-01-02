import { Routes, RouterModule } from "@angular/router";
import { AllocationReportComponent } from "./allocation/report/allocation-report.component";
import { AuthGuard } from "../../guards/auth.guard";
import { ResourceSearchComponent } from "./resources/search/resource-search.component";
import { ResourceDetailComponent } from "./resources/detail/resource-detail.component";
import { AddAllocationByResourceComponent } from "./allocation/add-by-resource/add-by-resource.component";

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
];

export const AllocationRouter = RouterModule.forChild(ALLOCATION_ROUTER);