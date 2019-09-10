import { RouterModule, Routes } from "@angular/router"
import { AuthGuard } from "../../guards/auth.guard";
import { AddContactsComponent } from "./contacts/add-contacts/add-contacts.component";
import { ListContactsComponent } from "./contacts/list-contacts/list-contacts.component";
import { DetailContactsComponent } from "./contacts/detail-contacts/detail-contacts.component";
import { SeniorityComponent } from "../admin/options/seniority";
import { SkillComponent } from "../admin/options/skill";
import { ProfileComponent } from "../admin/options/profile";
import { ReasonCauseComponent } from "../admin/options/reasonCause";
import { JobSearchComponent } from "./job-search/add/job-search-add";
import { JobSearchListComponent } from "./job-search/list/job-search-list";

const RECRUITMENT_ROUTER: Routes = [
    {
        path: "contacts",
        children: [
            { path: "add", component: AddContactsComponent, canActivate: [AuthGuard], data: {}},
            { path: "", component: ListContactsComponent, canActivate: [AuthGuard], data: {}},
            { path: ":id/detail", component: DetailContactsComponent, canActivate: [AuthGuard], data: {}}
        ]
    },

    { path: "seniorities", component: SeniorityComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "SENIORITY" } },

    { path: "skills", component: SkillComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "SKILL" } },
    
    { path: "perfiles", component: ProfileComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "PROFILE" } },

    { path: "reasonCauses", component: ReasonCauseComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "REASONCAUSE" } },

    {
        path: "jobSearch",
        children: [
            { path: "add", component: JobSearchComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "ADD-JOBSEARCH" }},
            { path: "", component: JobSearchListComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "SEARCH-JOBSEARCH" }},
        ]
    },
] 

export const RecruitmentRouter = RouterModule.forChild(RECRUITMENT_ROUTER)