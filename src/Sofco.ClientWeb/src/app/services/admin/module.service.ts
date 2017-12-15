import { Injectable } from '@angular/core';
import { Response, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class ModuleService {
    private baseUrl: string;

    constructor(private http: HttpAuth, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/modules`).map((res:Response) => res.json());
    }

    get(id: number) {
       return this.http.get(`${this.baseUrl}/modules/${id}`).map((res:Response) => res.json());
    }

    deactivate(id: number) {
        return this.http.put(`${this.baseUrl}/modules/${id}/active/false`, {}).map((res:Response) => res.json());
    }

    activate(id: number) {
        return this.http.put(`${this.baseUrl}/modules/${id}/active/true`, {}).map((res:Response) => res.json());
    }

    getOptions() {
        return this.http.get(`${this.baseUrl}/modules/options`).map((res:Response) => res.json());
    }

    getOptionsWithFunctionalities() {
        return this.http.get(`${this.baseUrl}/modules/modulesAndFunctionalities`).map((res:Response) => res.json());
    }

    edit(model) {
        return this.http.put(`${this.baseUrl}/modules`, model).map((res:Response) => res.json());
    }

    getFunctionalities(id: number) {
        return this.http.get(`${this.baseUrl}/modules/${id}/functionalities`).map((res:Response) => res.json());
    }
}