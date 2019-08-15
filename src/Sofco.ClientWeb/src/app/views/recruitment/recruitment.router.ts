import { RouterModule, Routes } from "@angular/router"
import { AuthGuard } from "../../guards/auth.guard";
import { AddContactsComponent } from "./contacts/add-contacts/add-contacts.component";
import { ListContactsComponent } from "./contacts/list-contacts/list-contacts.component";
import { DetailContactsComponent } from "./contacts/detail-contacts/detail-contacts.component";

const RECRUITMENT_ROUTER: Routes = [
    {
        path: "contacts",
        children: [
            { path: "add", component: AddContactsComponent, canActivate: [AuthGuard], data: {}},
            { path: "", component: ListContactsComponent, canActivate: [AuthGuard], data: {}},
            { path: ":id/detail", component: DetailContactsComponent, canActivate: [AuthGuard], data: {}}
        ]
    }
] 

export const RecruitmentRouter = RouterModule.forChild(RECRUITMENT_ROUTER)