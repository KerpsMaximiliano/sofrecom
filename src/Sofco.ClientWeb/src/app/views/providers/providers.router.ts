import { RouterModule, Routes } from "@angular/router";
import { AuthGuard } from "app/guards/auth.guard";
import { NotesAddComponent } from "./notes/add/notes-add";
import { NotesEditComponent } from "./notes/edit/notes-edit";
import { NotesComponent } from "./notes/list/notes";
import { NotesNoAccessComponent } from "./notes/no-access/notes-no-access";
import { ProvidersAddComponent } from "./providers/add/providers-add";
import { ProvidersEditComponent } from "./providers/edit/providers-edit";
import { ProvidersComponent } from "./providers/list/providers";


const PROVIDERS_ROUTER: Routes = [
    {
        path: 'providers',
        children: [
            { path: "", component: ProvidersComponent, canActivate: [AuthGuard] },
            { path: "add", component: ProvidersAddComponent, canActivate: [AuthGuard] },
            { path: "edit/:id", component: ProvidersEditComponent, canActivate: [AuthGuard] },
            { path: "notes/add", component: NotesAddComponent, canActivate: [AuthGuard] },
        ]
    },
    {
        path: 'notes',
        children: [
            { path: "", component: NotesComponent, canActivate: [AuthGuard] },
            { path: "add", component: NotesAddComponent, canActivate: [AuthGuard] },
            { path: "edit/:id", component: NotesEditComponent, canActivate: [AuthGuard] },
            { path: "no-access", component: NotesNoAccessComponent, canActivate: [AuthGuard] }
        ]
    }
]

export const ProvidersRouter = RouterModule.forChild(PROVIDERS_ROUTER);