import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";

@Injectable()
export class ModuleService {
    private baseUrl: string;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/modules`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    get(id: number) {
       return this.http.get(`${this.baseUrl}/modules/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    deactivate(id: number) {
        return this.http.put(`${this.baseUrl}/modules/${id}/active/false`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    activate(id: number) {
        return this.http.put(`${this.baseUrl}/modules/${id}/active/true`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    getOptions() {
        return this.http.get(`${this.baseUrl}/modules/options`, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    getOptionsWithFunctionalities() {
        return this.http.get(`${this.baseUrl}/modules/modulesAndFunctionalities`, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    edit(model) {
        return this.http.put(`${this.baseUrl}/modules`, model, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    getFunctionalities(id: number) {
        return this.http.get(`${this.baseUrl}/modules/${id}/functionalities`, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }
}