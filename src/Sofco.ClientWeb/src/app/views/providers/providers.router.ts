import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "app/guards/auth.guard";
import { IndustriesComponent } from "./industries/industries";
import { IndustryComponent } from "./industries/industry/industry";
import { NotesAddComponent } from "./notes/add/notes-add";
import { NotesEditComponent } from "./notes/edit/notes-edit";
import { NotesComponent } from "./notes/list/notes";
import { NotesNoAccessComponent } from "./notes/no-access/notes-no-access";
import { ProvidersAddComponent } from "./providers/add/providers-add";
import { ProvidersEditComponent } from "./providers/edit/providers-edit";
import { ProvidersComponent } from "./providers/list/providers";
import { PurchaseOrdersNew } from "./purchase-orders/edit/new-purcase-order/purchase-orders-new";
import { PurchaseOrdersEditComponent } from "./purchase-orders/edit/purchase-orders-edit";
import { PurchaseOrdersComponent } from "./purchase-orders/list/purchase-orders";
import { PurchaseOrdersNoAccessComponent } from "./purchase-orders/no-access/purchase-orders-no-access";

const PROVIDERS_ROUTER: Routes = [
  {
    path: "providers",
    children: [
      { path: "", component: ProvidersComponent, canActivate: [AuthGuard] },
      {
        path: "add",
        component: ProvidersAddComponent,
        canActivate: [AuthGuard],
      },
      {
        path: "edit/:id",
        component: ProvidersEditComponent,
        canActivate: [AuthGuard],
      },
      {
        path: "detail/:id",
        component: ProvidersEditComponent,
        canActivate: [AuthGuard],
      },
      {
        path: "notes/add",
        component: NotesAddComponent,
        canActivate: [AuthGuard],
      },
    ],
  },
  {
    path: "notes",
    children: [
      { path: "", component: NotesComponent, canActivate: [AuthGuard] },
      { path: "add", component: NotesAddComponent, canActivate: [AuthGuard] },
      {
        path: "edit/:id",
        component: NotesEditComponent,
        canActivate: [AuthGuard],
      },
      {
        path: "no-access",
        component: NotesNoAccessComponent,
        canActivate: [AuthGuard],
      },
    ],
  },
  {
    path: "purchase-orders",
    children: [
      {
        path: "",
        component: PurchaseOrdersComponent,
        canActivate: [AuthGuard],
      },
      { path: "nueva", component: PurchaseOrdersNew, canActivate: [AuthGuard] },
      {
        path: "edit/:id",
        component: PurchaseOrdersEditComponent,
        canActivate: [AuthGuard],
      },
      {
        path: "no-access",
        component: PurchaseOrdersNoAccessComponent,
        canActivate: [AuthGuard],
      },
    ],
  },
  {
    path: "industries",
    children: [
      {
        path: "",
        component: IndustriesComponent,
        canActivate: [AuthGuard],
      },
      {
        path: "industry/add",
        component: IndustryComponent,
        canActivate: [AuthGuard],
      },
      {
        path: "industry/view/:id",
        component: IndustryComponent,
        canActivate: [AuthGuard],
      },
      {
        path: "industry/edit/:id",
        component: IndustryComponent,
        canActivate: [AuthGuard],
      },
    ],
  },
];

export const ProvidersRouter = RouterModule.forChild(PROVIDERS_ROUTER);
