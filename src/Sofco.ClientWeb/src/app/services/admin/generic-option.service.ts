import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class GenericOptionService {
    private baseUrl: string;
    public controller: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get<any>(`${this.baseUrl}/${this.controller}`);
    }

    getOptions() {
       return this.http.get<any>(`${this.baseUrl}/${this.controller}/options`);
    }

    add(text){
        return this.http.post<any>(`${this.baseUrl}/${this.controller}`, { id: 0, text });
    }

    edit(id, text){
        return this.http.put<any>(`${this.baseUrl}/${this.controller}`, { id: id, text });
    }
    
    active(id, active){
        return this.http.put<any>(`${this.baseUrl}/${this.controller}/${id}/active/${active}`, {});
    }

}