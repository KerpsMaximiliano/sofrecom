import { Injectable } from '@angular/core';
import { SettingsService } from "../admin/settings.service";
import { AppSetting } from './app-setting';


@Injectable()
export class AppSettingService {

    private NumberType = "NUMBER";
    private baseUrl: string;

    constructor(private settingService: SettingsService,
        private appSetting: AppSetting) {
    }

    load(): Promise<any> {
        const result = this.settingService
                .getAll()
                .toPromise()
                .then(
                    d => {
                        this.loadHandler(d.body.data);

                        this.appSetting.ApiVersion = d.headers.get("x-app-version");

                        return this.appSetting;
                    }
                ).catch((err: any) => {
                    return Promise.resolve(null);
                });

        return result;
    }

    loadHandler(data: any) {
        const appSettting = this.appSetting;
        for (const key in appSettting) {
            const item = data.find(s => s.key === key);
            if (item !== undefined) {
                appSettting[key] = this.parseValueByType(item);
            }
        }
    }

    parseValueByType(item: any): any {
        if (item.type.toUpperCase() === this.NumberType) {
            return Number(item.value);
        }
        return item.value;
    }
}
