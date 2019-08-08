import { Injectable } from "@angular/core";

import { HttpClient } from '@angular/common/http';

import { Service } from "../common/service";

@Injectable()
export class DelegationService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    post(model) {
        return this.http.post<any>(`${this.baseUrl}/delegation`, model);
    }

    get() {
       return this.http.get<any>(`${this.baseUrl}/delegation`);
    }

    delete(id: number) {
        return this.http.delete<any>(`${this.baseUrl}/delegation/${id}`);
    }

    getAnalytics() {
        return this.http.get<any>(`${this.baseUrl}/delegation/analytics`);
     }
}