import { Injectable } from '@angular/core';
import { Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class FunctionalityService {
    private baseUrl: string;

    constructor(private http: HttpAuth, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/functionality`).map((res:Response) => res.json());
    }

    get(id: number) {
       return this.http.get(`${this.baseUrl}/functionality/${id}`).map((res:Response) => res.json());
    }

    deactivate(id: number) {
        return this.http.put(`${this.baseUrl}/functionality/${id}/active/false`, {}).map((res:Response) => res.json());
    }

    activate(id: number) {
        return this.http.put(`${this.baseUrl}/functionality/${id}/active/true`, {}).map((res:Response) => res.json());
    }

    getOptions() {
        return this.http.get(`${this.baseUrl}/functionality/options`).map((res:Response) => res.json());
    }
}