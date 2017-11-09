import { MessageService } from 'app/services/common/message.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { SettingsService } from "app/services/admin/settings.service";
import { AppSetting } from 'app/services/common/app-setting'

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html'
})
export class SettingsComponent implements OnInit, OnDestroy {
  
    data:Array<any>;
    serviceSubscrip: Subscription;
    public loading: boolean = false;

  constructor(
      private service: SettingsService,
      private messageService: MessageService,
      private appSettting: AppSetting,
      private errorHandlerService: ErrorHandlerService) {
  }
  
  ngOnInit() {
    this.getAll();
  }

  ngOnDestroy() {
    if(this.serviceSubscrip) this.serviceSubscrip.unsubscribe();
  }

  getAll() {
    this.loading = true;
    this.serviceSubscrip = this.service.getAll().subscribe(
      d => { this.data = d.json().data; },
      err => this.errorHandlerService.handleErrors(err),
      () => { this.loading = false; });
  }

  save() {
    this.loading = true;
    this.serviceSubscrip = this.service.save(this.data).subscribe(
      d => { this.data = d.data; this.saveHandler(); },
      err => this.errorHandlerService.handleErrors(err),
      () => { this.loading = false; });
  }

  saveHandler() {
    this.messageService.succes('ADMIN.settings.saveSuccess');
    for (var key in this.appSettting) {
      var item = this.getValueByKey(key);
      if(item != null){
        this.appSettting[key] = item;
      }
    }
  }

  getValueByKey(key:string) {
    let setting = this.data.find(s => s.key == key);
    return setting != undefined?setting.value:null;
  }
}