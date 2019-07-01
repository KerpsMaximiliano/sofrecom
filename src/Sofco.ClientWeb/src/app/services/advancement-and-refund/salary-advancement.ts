import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class SalaryAdvancementService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    get(){
        return this.http.get<any>(`${this.baseUrl}/salaryAdvancement`);
    }

    add(json): any {
        return this.http.post<any>(`${this.baseUrl}/salaryAdvancement`, json);
    }

    delete(id): any {
        return this.http.delete<any>(`${this.baseUrl}/salaryAdvancement/${id}`);
    }
}