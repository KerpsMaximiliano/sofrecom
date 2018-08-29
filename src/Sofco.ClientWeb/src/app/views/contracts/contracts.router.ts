import { Routes, RouterModule } from "@angular/router";
import { AnalyticSearchComponent } from "./analytics/search/analytic-search.component";
import { NewAnalyticComponent } from "./analytics/new/new-analytic.component";
import { EditAnalyticComponent } from "./analytics/edit/edit-analytic.component";
import { AddAllocationComponent } from "../allocation-management/allocation/add-by-analytic/add-by-analytic.component";
import { ResourceByAnalyticComponent } from "../allocation-management/resources/by-analytic/resource-by-analytic.component";
import { ViewAnalyticComponent } from "./analytics/view/view-analytic.component";
import { ListCostCenterComponent } from "./cost-center/list/list-cost-center.component";
import { AddCostCenterComponent } from "./cost-center/add/add-cost-center.component";
import { EditCostCenterComponent } from "./cost-center/edit/edit-cost-center.component";
import { AuthGuard } from "../../guards/auth.guard";

const CONTRACTS_ROUTER: Routes = [
    { path: "analytics",
        children: [
        { path: "", component: AnalyticSearchComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "QUERY" } },
        { path: "new", component: NewAnalyticComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "ANADD" } },
        { path: ":id/edit", component: EditAnalyticComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "ANEDT" } },
        { path: ":id/view", component: ViewAnalyticComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "QUERY" } },
        { path: ":id/allocations", component: AddAllocationComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "ADRES" } },
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