import { Routes, RouterModule } from "@angular/router";
import { AnalyticSearchComponent } from "app/views/contracts/analytics/search/analytic-search.component";
import { NewAnalyticComponent } from "app/views/contracts/analytics/new/new-analytic.component";
import { EditAnalyticComponent } from "app/views/contracts/analytics/edit/edit-analytic.component";
import { AddAllocationComponent } from "app/views/allocation-management/allocation/add-by-analytic/add-by-analytic.component";
import { ResourceByAnalyticComponent } from "app/views/allocation-management/resources/by-analytic/resource-by-analytic.component";
import { ViewAnalyticComponent } from "app/views/contracts/analytics/view/view-analytic.component";
import { ListCostCenterComponent } from "app/views/contracts/cost-center/list/list-cost-center.component";
import { AddCostCenterComponent } from "app/views/contracts/cost-center/add/add-cost-center.component";
import { EditCostCenterComponent } from "app/views/contracts/cost-center/edit/edit-cost-center.component";
import { AuthGuard } from "app/guards/auth.guard";

const CONTRACTS_ROUTER: Routes = [
    { path: "analytics",
        children: [
        { path: "", component: AnalyticSearchComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "QUERY" } },
        { path: "new", component: NewAnalyticComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "ANADD" } },
        { path: ":id/edit", component: EditAnalyticComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "ANEDT" } },
        { path: ":id/view", component: ViewAnalyticComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "QUERY" } },
        { path: ":id/allocations", component: AddAllocationComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "ADRES" } },
        { path: ":id/resources", component: ResourceByAnalyticComponent, canActivate: [AuthGuard] },
    ]},
    {
      path: "costCenter",
      children: [
        { path: "", component: ListCostCenterComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "CCLST" } },
        { path: "add", component: AddCostCenterComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "CCADD" } },
        { path: ":id/edit", component: EditCostCenterComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "CCADD" } }
      ]
    }
];

export const ContractsRouter = RouterModule.forChild(CONTRACTS_ROUTER);