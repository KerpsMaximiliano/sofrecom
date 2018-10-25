import { MessageService } from '../../../services/common/message.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { SettingsService } from '../../../services/admin/settings.service';
import { AppSettingService } from '../../../services/common/app-setting.service';
import { AppSetting } from '../../../services/common/app-setting';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html'
})
export class SettingsComponent implements OnInit, OnDestroy {

  JobsSettingCategory = 2;

  settings: Array<any>;
  serviceSubscrip: Subscription;
  licenseTypesSubscrip: Subscription;
  public loading = false;

  licenseTypes: Array<any> = new Array();
  jobSettings: Array<any> = new Array();

  constructor(
      private service: SettingsService,
      private messageService: MessageService,
      private appSettting: AppSetting,
      private appSetttingService: AppSettingService) {
  }

  ngOnInit() {
    this.getAll();
    this.getLicenseTypes();
  }

  ngOnDestroy() {
    if (this.serviceSubscrip) { this.serviceSubscrip.unsubscribe(); }
    if (this.licenseTypesSubscrip) { this.licenseTypesSubscrip.unsubscribe(); }
  }

  getAll() {
    this.loading = true;
    this.serviceSubscrip = this.service.getAll().subscribe(
      d => {
        const settings: Array<any> = d.body.data;
        this.settings = settings.filter(item => item.category !== this.JobsSettingCategory);
        this.jobSettings = settings.filter(item => item.category === this.JobsSettingCategory);
        this.loading = false
      },
      err => this.loading = false);
  }

  getLicenseTypes() {
    this.messageService.showLoading();

    this.licenseTypesSubscrip = this.service.getLicenseTypes().subscribe(
      response => { 
        this.licenseTypes = response; 
        this.messageService.closeLoading(); 
      },
      err => this.messageService.closeLoading());
  }

  save() {
    this.loading = true;

    this.serviceSubscrip = this.service.save(this.settings).subscribe(
      d => { this.settings = d.data; this.loading = false; this.saveResponseHandler(); },
      err => this.loading = false);
  }

  updateLicenseType(item) {
    this.messageService.showLoading();

    this.licenseTypesSubscrip = this.service.saveLicenseType(item).subscribe(
      response => { this.messageService.closeLoading(); },
      err => this.messageService.closeLoading());
  }

  saveResponseHandler() {
    this.messageService.succes('ADMIN.settings.saveSuccess');
    for (const key in this.appSettting) {

      const item = this.settings.find(s => s.key === key);

      if (item != null) {
        this.appSettting[key] = this.appSetttingService.parseValueByType(item);
      }
    }
  }

  saveSetting(item) {
    this.messageService.showLoading();

    this.licenseTypesSubscrip = this.service.saveItem(item).subscribe(
      response => { this.messageService.closeLoading(); },
      err => this.messageService.closeLoading());
  }
}
