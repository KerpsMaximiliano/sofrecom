import { MessageService } from '../../../services/common/message.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { SettingsService } from '../../../services/admin/settings.service';
import { AppSettingService } from '../../../services/common/app-setting.service';
import { AppSetting } from '../../../services/common/app-setting';
import { UserService } from 'app/services/admin/user.service';
import { forEach } from 'lodash';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit, OnDestroy {

  

  settings: Array<any>;
  serviceSubscrip: Subscription;
  licenseTypesSubscrip: Subscription;
  public loading = false;

  licenseTypes: Array<any> = new Array();
  jobSettings: Array<any> = new Array();

  settingUsers: number[] = [];
  usuarios: any[] = new Array<any>();

  constructor(
      private service: SettingsService,
      private messageService: MessageService,
      private appSettting: AppSetting,
      private appSetttingService: AppSettingService,
      private userService: UserService) {
  }

  ngOnInit() {
    this.getAll();
    this.getLicenseTypes();
    this.getUsers();
  }

  ngOnDestroy() {
    if (this.serviceSubscrip) { this.serviceSubscrip.unsubscribe(); }
    if (this.licenseTypesSubscrip) { this.licenseTypesSubscrip.unsubscribe(); }
  }

  getUsers() {
    this.messageService.showLoading();
    this.userService.getAll().subscribe(d=>{
      d.forEach(user => {
        this.usuarios.push({id:user.id, name: user.name})});
      this.usuarios = [...this.usuarios]
      this.messageService.closeLoading()});
  }

  getAll() {
    this.loading = true;
    let JobsSettingCategory = 2;
    this.serviceSubscrip = this.service.getAll().subscribe(
      d => {
        const settings: Array<any> = d.body.data;
        this.settings = settings.filter(item => item.category !== JobsSettingCategory);
        this.jobSettings = settings.filter(item => item.category === JobsSettingCategory);
        this.settingUsers = JSON.parse(this.jobSettings[2].value);
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

  save(item) {
    this.loading = true;
    this.serviceSubscrip = this.service.saveItem(item).subscribe(
      (res) => {
        this.messageService.closeLoading();
        this.messageService.succes('ADMIN.settings.saveSuccess');
        this.loading = false;
      },
      (error) => {
        this.messageService.closeLoading();
        this.messageService.showError('Ha ocurrido un error al guardar los datos.');
      }
    );
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
    if (item.value < 1 || item.value > 31) {
      this.messageService.showMessage("El día del mes de Job de Recurso SIN Asignación debe ser del 1 al 31", 1);
      this.messageService.closeLoading();
    } else{
      if(item.id === 26){
        item.value = JSON.stringify(this.settingUsers)
      }
    this.licenseTypesSubscrip = this.service.saveItem(item).subscribe(
        (res) => {
          this.messageService.closeLoading();
          this.messageService.succes('ADMIN.settings.saveSuccess');
        },
        (error) => {
          this.messageService.closeLoading();
          this.messageService.showError('Ha ocurrido un error al guardar los datos.');
        }
    );
    }
  }

  // saveSetting2(item) {
  //   if (item < 1 || item > 31) {
  //     this.messageService.showMessage("El día del mes de Job de Recurso SIN Asignación debe ser del 1 al 31", 1);
  //   } else {
  //     this.service.put({ key: "JobRecursosSinAsignacionDia", value: item}).subscribe(
  //       response => { 
  //         this.messageService.closeLoading();
  //         this.messageService.succes('ADMIN.settings.saveSuccess');
  //     },
  //     err => this.messageService.closeLoading());
  //   }
  // } 

  // saveSetting3(item) {
  //   this.service.put({ key: "JobRecursosSinAsignacionDestinatarios", value: JSON.stringify(item)}).subscribe(
  //     response => { 
  //       this.messageService.closeLoading();
  //       this.messageService.succes('ADMIN.settings.saveSuccess');
  //   },
  //   err => this.messageService.closeLoading());
  // }
}
