import { TranslateModule } from '@ngx-translate/core';
import {NgModule} from "@angular/core";
import {RouterModule} from "@angular/router";

import {BsDropdownModule} from 'ngx-bootstrap';

import {BasicLayoutComponent} from "./basicLayout.component";
import {BlankLayoutComponent} from "./blankLayout.component";

import {NavigationComponent} from "./../navigation/navigation.component";
import {FooterComponent} from "./../footer/footer.component";
import {TopNavbarComponent} from "./../topnavbar/topnavbar.component";
import { NgxInactivity } from 'ngx-inactivity';
import { Ng2ModalModule } from 'app/components/modal/ng2modal.module';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    FooterComponent,
    BasicLayoutComponent,
    BlankLayoutComponent,
    NavigationComponent,
    TopNavbarComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    NgxInactivity, 
    Ng2ModalModule,
    BsDropdownModule.forRoot(),
    TranslateModule
  ],
  exports: [
    FooterComponent,
    BasicLayoutComponent,
    BlankLayoutComponent,
    NavigationComponent,
    TopNavbarComponent
  ],
})

export class LayoutsModule {}
