import { Injectable } from '@angular/core';
import { Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class SettingsService {

    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get<any>(`${this.baseUrl}/settings`, {observe: 'response'});
    }

    save (data: any) {
        return this.http.post<any>(`${this.baseUrl}/settings`, data);
    }

    put (data: any) {
        return this.http.put<any>(`${this.baseUrl}/settings`, data);
    }

    saveItem (data: any) {
        return this.http.post<any>(`${this.baseUrl}/settings/${data.id}`, data);
    }

    getLicenseTypes() {
        return this.http.get<any>(`${this.baseUrl}/settings/licenseTypes`);
    }

    saveLicenseType(item) {
        return this.http.put<any>(`${this.baseUrl}/licenses/type/${item.typeId}/days/${item.value}`, {});
    }
}
