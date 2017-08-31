import { Group } from 'models/group';
import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/service";

@Injectable()
export class GroupService {
    private baseUrl: string;
    private headers: Headers;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
        this.headers = this.service.getHeaders();
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/group`, { headers: this.headers}).map((res:Response) => res.json());
    }

    get(id: number) {
       return this.http.get(`${this.baseUrl}/group/${id}`, { headers: this.headers}).map((res:Response) => res.json());
    }

    add(model : Group) {
        return this.http.post(`${this.baseUrl}/group`, model, { headers: this.headers}).map((res:Response) => res.json());
    }

    edit(model) {
        return this.http.put(`${this.baseUrl}/group`, model, { headers: this.headers}).map((res:Response) => res.json() );
    }

    delete(id: number) {
        return this.http.delete(`${this.baseUrl}/group/${id}`, { headers: this.headers}).map((res:Response) => res.json() );
    }

    deactivate(id: number) {
        return this.http.put(`${this.baseUrl}/group/${id}/active/false`, {}, { headers: this.headers}).map((res:Response) => res.json() );
    }

    activate(id: number) {
        return this.http.put(`${this.baseUrl}/group/${id}/active/true`, {}, { headers: this.headers}).map((res:Response) => res.json() );
    }

    getOptions() {
        return this.http.get(`${this.baseUrl}/group/options`, { headers: this.headers}).map((res:Response) => res.json() );
    }
}