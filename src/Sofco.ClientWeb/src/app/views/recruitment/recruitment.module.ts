import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListContactsComponent } from './contacts/list-contacts/list-contacts.component';
import { AddContactsComponent } from './contacts/add-contacts/add-contacts.component';
import { DetailContactsComponent } from './contacts/detail-contacts/detail-contacts.component';
import { RouterModule } from '@angular/router';
import { RecruitmentRouter } from './recruitment.router';
import { SkillComponent } from '../admin/options/skill';
import { ProfileComponent } from '../admin/options/profile';
import { SeniorityComponent } from '../admin/options/seniority';
import { GenericOptionService } from 'app/services/admin/generic-option.service';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ICheckModule } from 'app/components/icheck/icheck.module';
import { Ng2ModalModule } from 'app/components/modal/ng2modal.module';
import { TranslateModule } from '@ngx-translate/core';
import { NgSelectModule } from '@ng-select/ng-select';
import { ReasonCauseComponent } from '../admin/options/reasonCause';
import { JobSearchService } from 'app/services/recruitment/jobsearch.service';
import { JobSearchComponent } from './job-search/add/job-search-add';
import { FormsService } from 'app/services/forms/forms.service';
import { CustomerService } from 'app/services/billing/customer.service';
import { DecimalFormatModule } from 'app/components/decimalFormat/decimal-format.directive';
import { DigitModule } from 'app/components/digit-limit/digit-limit.directive';
import { JobSearchListComponent } from './job-search/list/job-search-list';
import { AmountFormatModule } from 'app/pipes/amount-format.module';
import { JobSearchEditComponent } from './job-search/edit/job-search-edit';
import { BsDatepickerModule } from 'ngx-bootstrap';
import { TimeHiringComponent } from '../admin/options/timeHiring';
import { ApplicantService } from 'app/services/recruitment/applicant.service';
import { ApplicantsRelatedComponent } from './job-search/applicants-related/applicants-related';
import { ResourceAssignmentComponent } from '../admin/options/resourceAssignment';

@NgModule({
  declarations: [
    ListContactsComponent, 
    AddContactsComponent, 
    DetailContactsComponent, 
    SkillComponent, 
    ProfileComponent, 
    TimeHiringComponent,
    SeniorityComponent, 
    ReasonCauseComponent,
    JobSearchComponent,
    JobSearchListComponent,
    JobSearchEditComponent,
    ApplicantsRelatedComponent,
    ResourceAssignmentComponent
  ],

  imports: [
    CommonModule, 
    RouterModule, 
    RecruitmentRouter, 
    FormsModule, 
    AmountFormatModule,
    ICheckModule, 
    Ng2ModalModule, 
    TranslateModule, 
    DigitModule,
    NgSelectModule,  
    DecimalFormatModule,
    ReactiveFormsModule,
    BsDatepickerModule
  ],

  providers: [GenericOptionService, JobSearchService, FormsService, CustomerService, ApplicantService],
  exports: []
})

export class RecruitmentModule { }