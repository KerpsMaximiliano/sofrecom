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
import { DatePickerModule } from 'app/components/date-picker/date-picker.module';
import { NgSelectModule } from '@ng-select/ng-select';

@NgModule({
  declarations: [
    ListContactsComponent, AddContactsComponent, DetailContactsComponent, SkillComponent, ProfileComponent, SeniorityComponent
  ],

  imports: [CommonModule, RouterModule, RecruitmentRouter, FormsModule, ICheckModule, Ng2ModalModule, TranslateModule, DatePickerModule, NgSelectModule, ReactiveFormsModule],

  providers: [GenericOptionService],
  exports: []
})

export class RecruitmentModule { }