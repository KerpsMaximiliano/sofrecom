import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListContactsComponent } from './contacts/list-contacts/list-contacts.component';
import { AddContactsComponent } from './contacts/add-contacts/add-contacts.component';
import { DetailContactsComponent } from './contacts/detail-contacts/detail-contacts.component';
import { RouterModule } from '@angular/router';
import { RecruitmentRouter } from './recruitment.router';
import { Select2Module } from 'app/components/select2/select2';
import { NgSelectModule } from '@ng-select/ng-select';
import { TranslateModule } from '@ngx-translate/core';

@NgModule({
  declarations: [
    ListContactsComponent, AddContactsComponent, DetailContactsComponent
  ],
  imports: [
    CommonModule, RouterModule, RecruitmentRouter, TranslateModule, Select2Module, NgSelectModule
  ],
  providers: [],
  exports: []
})

export class RecruitmentModule { }
