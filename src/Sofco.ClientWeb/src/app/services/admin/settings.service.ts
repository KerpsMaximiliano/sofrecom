import { Injectable } from '@angular/core';
import { Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class SettingsService {

    private baseUrl: string;

    constructor(private http: HttpAuth, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/settings`);
    }

    save (data: any) {
        return this.http.post(`${this.baseUrl}/settings`, data).map((res: Response) => res.json());
    }

    getLicenseTypes() {
        return this.http.get(`${this.baseUrl}/settings/licenseTypes`).map((res: Response) => res.json());
    }

    saveLicenseType(item){
        return this.http.put(`${this.baseUrl}/licenses/type/${item.typeId}/days/${item.value}`, {}).map((res: Response) => res.json());
    }
}