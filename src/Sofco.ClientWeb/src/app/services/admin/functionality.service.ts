import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class FunctionalityService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get<any>(`${this.baseUrl}/functionality`);
    }

    get(id: number) {
       return this.http.get<any>(`${this.baseUrl}/functionality/${id}`);
    }

    deactivate(id: number) {
        return this.http.put<any>(`${this.baseUrl}/functionality/${id}/active/false`, {});
    }

    activate(id: number) {
        return this.http.put<any>(`${this.baseUrl}/functionality/${id}/active/true`, {});
    }

    getOptions() {
        return this.http.get<any>(`${this.baseUrl}/functionality/options`);
    }
}