import { Injectable } from '@angular/core';
import { Response, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ModuleService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get<any>(`${this.baseUrl}/modules`);
    }

    get(id: number) {
       return this.http.get<any>(`${this.baseUrl}/modules/${id}`);
    }

    deactivate(id: number) {
        return this.http.put<any>(`${this.baseUrl}/modules/${id}/active/false`, {});
    }

    activate(id: number) {
        return this.http.put<any>(`${this.baseUrl}/modules/${id}/active/true`, {});
    }

    getOptions() {
        return this.http.get<any>(`${this.baseUrl}/modules/options`);
    }

    getOptionsWithFunctionalities() {
        return this.http.get<any>(`${this.baseUrl}/modules/modulesAndFunctionalities`);
    }

    edit(model) {
        return this.http.put<any>(`${this.baseUrl}/modules`, model);
    }

    getFunctionalities(id: number) {
        return this.http.get<any>(`${this.baseUrl}/modules/${id}/functionalities`);
    }
}