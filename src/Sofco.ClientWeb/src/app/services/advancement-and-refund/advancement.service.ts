import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AdvancementService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    add(model){
        return this.http.post<any>(`${this.baseUrl}/advancement`, model);
    }

    edit(model){
        return this.http.put<any>(`${this.baseUrl}/advancement`, model);
    }

    get(id){
        return this.http.get<any>(`${this.baseUrl}/advancement/${id}`);
    }

    getHistories(id){
        return this.http.get<any>(`${this.baseUrl}/advancement/${id}/histories`);
    }

    getAllInProcess(){
        return this.http.get<any>(`${this.baseUrl}/advancement/inProcess`);
    }
}