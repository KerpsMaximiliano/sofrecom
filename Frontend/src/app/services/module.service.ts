import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/service";

@Injectable()
export class ModuleService {
    private baseUrl: string;
    private headers: Headers;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
        this.headers = this.service.getHeaders();
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/module`, { headers: this.headers}).map((res:Response) => res.json());
    }

    get(id: number) {
       return this.http.get(`${this.baseUrl}/module/${id}`, { headers: this.headers}).map((res:Response) => res.json());
    }

    deactivate(id: number) {
        return this.http.put(`${this.baseUrl}/module/${id}/active/false`, {}, { headers: this.headers}).map((res:Response) => res.json() );
    }

    activate(id: number) {
        return this.http.put(`${this.baseUrl}/module/${id}/active/true`, {}, { headers: this.headers}).map((res:Response) => res.json() );
    }

    getOptions() {
        return this.http.get(`${this.baseUrl}/module/options`, { headers: this.headers}).map((res:Response) => res.json() );
    }

    edit(model) {
        return this.http.put(`${this.baseUrl}/module`, model, { headers: this.headers}).map((res:Response) => res.json() );
    }
    
    assignFunctionalities(moduleId: number, objToSend: any){
        return this.http.post(`${this.baseUrl}/module/${moduleId}/functionalities`, objToSend, { headers: this.headers}).map((res:Response) => res.json() );
    }

    unAssignFunctionality(moduleId: number, funcId: number) {
        return this.http.delete(`${this.baseUrl}/module/${moduleId}/functionality/${funcId}`, { headers: this.headers}).map((res:Response) => res.json() );
    }

    unAssignFunctionalities(moduleId: number, objToSend: any){
        return this.http.post(`${this.baseUrl}/module/${moduleId}/functionalities`, objToSend, { headers: this.headers}).map((res:Response) => res.json() );
    }
}