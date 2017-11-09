import { Injectable } from '@angular/core';
import { SettingsService } from "app/services/admin/settings.service";
import { AppSetting } from 'app/services/common/app-setting'
import 'rxjs/add/operator/toPromise';

@Injectable()
export class AppSettingService {
    private baseUrl: string;
    constructor(private settingService: SettingsService, 
        private appSetting: AppSetting) {
    }

    load():Promise<any> {
        let result = 
            this.settingService
                .getAll()
                .toPromise()
                .then(
                    d => { 
                        this.loadHandler(d.json().data); 
                        return this.appSetting;
                    }
                ).catch((err: any) => {
                    return Promise.resolve(null);
                });

        return result;
    }

    loadHandler(data:any) {
        let appSettting = this.appSetting;
        for (var key in appSettting) {
            var item = data.find(s => s.key == key);
            if(item != undefined){ appSettting[key] = item.value; }
        }
    }
}