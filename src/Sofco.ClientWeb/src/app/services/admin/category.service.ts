import { Injectable } from '@angular/core';
import { Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class CategoryService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get<any>(`${this.baseUrl}/category`);
    }

    get(id: number) {
       return this.http.get<any>(`${this.baseUrl}/category/${id}`);
    }

    add(description){
        return this.http.post<any>(`${this.baseUrl}/category`, { description: description });
    }

    edit(model){
        return this.http.put<any>(`${this.baseUrl}/category`, model);
    }
    
    active(id, active){
        return this.http.put<any>(`${this.baseUrl}/category/${id}/active/${active}`, {});
    }

    getOptions() {
        return this.http.get<any>(`${this.baseUrl}/category/options`);
    }
}