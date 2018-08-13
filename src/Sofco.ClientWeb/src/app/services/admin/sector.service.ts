import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class SectorService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get<any>(`${this.baseUrl}/sectors`);
    }

    get(id: number) {
       return this.http.get<any>(`${this.baseUrl}/sectors/${id}`);
    }

    add(text, responsableId){
        return this.http.post<any>(`${this.baseUrl}/sectors`, { text, responsableId });
    }

    edit(model){
        return this.http.put<any>(`${this.baseUrl}/sectors`, model);
    }
    
    active(id, active){
        return this.http.put<any>(`${this.baseUrl}/sectors/${id}/active/${active}`, {});
    }
}