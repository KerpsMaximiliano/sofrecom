import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class CurrentAccountService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    get(){
        return this.http.get<any>(`${this.baseUrl}/currentAccount`);
    }

    updateMassive(json): any {
        return this.http.put<any>(`${this.baseUrl}/currentAccount`, json);
    }
}