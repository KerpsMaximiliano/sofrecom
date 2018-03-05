import { MessageService } from 'app/services/common/message.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { SettingsService } from "app/services/admin/settings.service";
import { AppSettingService } from 'app/services/common/app-setting.service'
import { AppSetting } from 'app/services/common/app-setting'

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html'
})
export class SettingsComponent implements OnInit, OnDestroy {
  
    data:Array<any>;
    serviceSubscrip: Subscription;
    licenseTypesSubscrip: Subscription;
    public loading: boolean = false;

    licenseTypes: Array<any> = new Array();

  constructor(
      private service: SettingsService,
      private messageService: MessageService,
      private appSettting: AppSetting,
      private appSetttingService: AppSettingService,
      private errorHandlerService: ErrorHandlerService) {
  }
  
  ngOnInit() {
    this.getAll();
    this.getLicenseTypes();
  }

  ngOnDestroy() {
    if(this.serviceSubscrip) this.serviceSubscrip.unsubscribe();
    if(this.licenseTypesSubscrip) this.licenseTypesSubscrip.unsubscribe();
  }

  getAll() {
    this.loading = true;
    this.serviceSubscrip = this.service.getAll().subscribe(
      d => { this.data = d.json().data; },
      err => this.errorHandlerService.handleErrors(err),
      () => { this.loading = false; });
  }

  getLicenseTypes(){
    this.messageService.showLoading();

    this.licenseTypesSubscrip = this.service.getLicenseTypes().subscribe(
      response => {
        this.licenseTypes = response;
      },
      err => {},
      () => this.messageService.closeLoading()
    )
  }

  save() {
    this.loading = true;
    this.serviceSubscrip = this.service.save(this.data).subscribe(
      d => { this.data = d.data; this.saveHandler(); },
      err => this.errorHandlerService.handleErrors(err),
      () => { this.loading = false; });
  }

  updateLicenseType(item){
    this.messageService.showLoading();
    
    this.licenseTypesSubscrip = this.service.saveLicenseType(item).subscribe(
      response => { 
        if(response.messages) this.messageService.showMessages(response.messages);
      },
      err => this.errorHandlerService.handleErrors(err),
      () => this.messageService.closeLoading());
  }

  saveHandler() {
    this.messageService.succes('ADMIN.settings.saveSuccess');
    for (var key in this.appSettting) {
      var item = this.data.find(s => s.key == key);
      if(item != null){
        this.appSettting[key] = this.appSetttingService.parseValueByType(item);
      }
    }
  }
}