import { Component } from '@angular/core';
import { AppSetting } from '../../../services/common/app-setting'
declare function require(name: string);

@Component({
  selector: 'app-footer',
  templateUrl: 'footer.template.html',
  styleUrls: ['./footer.component.scss']
})

export class FooterComponent {
  public appVersion: string;
  public apiVersion: string;

  constructor(private appSetting: AppSetting) {
    const appPackage = require(`../../../../../package.json`);

    this.appVersion = appPackage.version;

    this.apiVersion = appSetting.ApiVersion;
  }
}
