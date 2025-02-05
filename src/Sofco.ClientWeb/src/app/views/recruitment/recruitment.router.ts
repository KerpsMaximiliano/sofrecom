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
import { JobSearchEditComponent } from "./job-search/edit/job-search-edit";
import { TimeHiringComponent } from "../admin/options/timeHiring";
import { ResourceAssignmentComponent } from "../admin/options/resourceAssignment";
import { RecruitmentReportComponent } from "./reports/recruitment-report";

const RECRUITMENT_ROUTER: Routes = [
    {
        path: "contacts",
        children: [
            { path: "add", component: AddContactsComponent, canActivate: [AuthGuard], data: {  module: "RECRU", functionality: "ADD-CANDIDATE" }},
            { path: "", component: ListContactsComponent, canActivate: [AuthGuard], data: {  module: "RECRU", functionality: "SEARCH-CANDIDATE" }},
            { path: ":id", component: DetailContactsComponent, canActivate: [AuthGuard], data: {  module: "RECRU", functionality: "EDIT-CANDIDATE" }}
        ]
    },

    { path: "seniorities", component: SeniorityComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "SENIORITY" } },

    { path: "skills", component: SkillComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "SKILL" } },
    
    { path: "perfiles", component: ProfileComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "PROFILE" } },

    { path: "reasonCauses", component: ReasonCauseComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "REASONCAUSE" } },
    
    { path: "timeHirings", component: TimeHiringComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "TIMEHIRING" } },
    
    { path: "resourceAssignments", component: ResourceAssignmentComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "RESOURCEASSIGNMENT" } },

    { path: "report", component: RecruitmentReportComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "REPORT" }},
    
    {
        path: "jobSearch",
        children: [
            { path: "add", component: JobSearchComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "ADD-JOBSEARCH" }},
            { path: "", component: JobSearchListComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "SEARCH-JOBSEARCH" }},
            { path: ":id", component: JobSearchEditComponent, canActivate: [AuthGuard], data: { module: "RECRU", functionality: "EDIT-JOBSEARCH" }},
        ]
    },
] 

export const RecruitmentRouter = RouterModule.forChild(RECRUITMENT_ROUTER)