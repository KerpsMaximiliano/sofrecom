import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";

@Injectable()
export class FunctionalityService {
    private baseUrl: string;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/functionality`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    get(id: number) {
       return this.http.get(`${this.baseUrl}/functionality/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    deactivate(id: number) {
        return this.http.put(`${this.baseUrl}/functionality/${id}/active/false`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    activate(id: number) {
        return this.http.put(`${this.baseUrl}/functionality/${id}/active/true`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    getOptions() {
        return this.http.get(`${this.baseUrl}/functionality/options`, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }
}