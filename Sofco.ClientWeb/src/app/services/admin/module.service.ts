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
        return this.http.get(`${this.baseUrl}/module`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    get(id: number) {
       return this.http.get(`${this.baseUrl}/module/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    deactivate(id: number) {
        return this.http.put(`${this.baseUrl}/module/${id}/active/false`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    activate(id: number) {
        return this.http.put(`${this.baseUrl}/module/${id}/active/true`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    getOptions() {
        return this.http.get(`${this.baseUrl}/module/options`, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    getOptionsWithFunctionalities() {
        return this.http.get(`${this.baseUrl}/module/modulesAndFunctionalities`, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    edit(model) {
        return this.http.put(`${this.baseUrl}/module`, model, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    getFunctionalities(id: number) {
        return this.http.get(`${this.baseUrl}/module/${id}/functionalities`, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }
}