import { Injectable } from '@angular/core';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AreaService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get<any>(`${this.baseUrl}/areas`);
    }

    get(id: number) {
       return this.http.get<any>(`${this.baseUrl}/areas/${id}`);
    }

    add(text, responsableId){
        return this.http.post<any>(`${this.baseUrl}/areas`, { text, responsableId });
    }

    edit(model){
        return this.http.put<any>(`${this.baseUrl}/areas`, model);
    }
    
    active(id, active){
        return this.http.put<any>(`${this.baseUrl}/areas/${id}/active/${active}`, {});
    }

}