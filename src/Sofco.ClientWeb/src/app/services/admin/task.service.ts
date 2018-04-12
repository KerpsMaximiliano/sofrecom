import { Injectable } from '@angular/core';
import { Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class TaskService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get<any>(`${this.baseUrl}/task`);
    }

    get(id: number) {
       return this.http.get<any>(`${this.baseUrl}/task/${id}`);
    }

    add(description, categoryId){
        return this.http.post<any>(`${this.baseUrl}/task`, { description: description, categoryId: categoryId });
    }

    edit(model){
        return this.http.put<any>(`${this.baseUrl}/task`, model);
    }
    
    active(id, active){
        return this.http.put<any>(`${this.baseUrl}/task/${id}/active/${active}`, {});
    }

    getOptions() {
        return this.http.get<any>(`${this.baseUrl}/task/options`);
    }
}