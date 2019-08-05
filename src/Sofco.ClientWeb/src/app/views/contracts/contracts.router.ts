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
import { AddCloseDateComponent } from "app/views/contracts/closeDates/add/closeDate-add.component";
import { CurrencyExchangeComponent } from "./currency-exchange/currency-exchange";
import { ResourceByServiceComponent } from "../allocation-management/resources/by-service/resource-by-service.component";

const CONTRACTS_ROUTER: Routes = [
    { path: "analytics",
        children: [
        { path: "", component: AnalyticSearchComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "QUERY" } },
        { path: "new", component: NewAnalyticComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "ANADD" } },
        { path: ":id/edit", component: EditAnalyticComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "ANEDT" } },
        { path: ":id/view", component: ViewAnalyticComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "QUERY" } },
        { path: ":id/allocations", component: AddAllocationComponent, canActivate: [AuthGuard], data: { module: "ALLOC", functionality: "ADRES" } },
        { path: ":id/resources", component: ResourceByServiceComponent, canActivate: [AuthGuard] },
    ]},
    
    {
      path: "costCenter",
      children: [
        { path: "", component: ListCostCenterComponent, canActivate: [AuthGuard], data: { module: "COSTC", functionality: "QUERY" } },
        { path: "add", component: AddCostCenterComponent, canActivate: [AuthGuard], data: { module: "COSTC", functionality: "ADD" } },
        { path: ":id/edit", component: EditCostCenterComponent, canActivate: [AuthGuard], data: { module: "COSTC", functionality: "UPDAT" } }
      ]
    },

    { path: "closeDate",  component: AddCloseDateComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "CDADD" } },

    { path: "currencyExchange",  component: CurrencyExchangeComponent, canActivate: [AuthGuard], data: { module: "CONTR", functionality: "CURRENCY-EXCHANGE" } },
];

export const ContractsRouter = RouterModule.forChild(CONTRACTS_ROUTER);


