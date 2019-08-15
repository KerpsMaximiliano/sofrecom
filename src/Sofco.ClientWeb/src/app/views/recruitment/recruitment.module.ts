import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ListContactsComponent } from './contacts/list-contacts/list-contacts.component';
import { AddContactsComponent } from './contacts/add-contacts/add-contacts.component';
import { DetailContactsComponent } from './contacts/detail-contacts/detail-contacts.component';
import { RouterModule } from '@angular/router';
import { RecruitmentRouter } from './recruitment.router';

@NgModule({
  declarations: [
    ListContactsComponent, AddContactsComponent, DetailContactsComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    RecruitmentRouter
  ],
  providers: [],
  exports: []
})

export class RecruitmentModule { }
