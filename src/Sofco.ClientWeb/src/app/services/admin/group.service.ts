import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";
import { Group } from "app/models/admin/group";

@Injectable()
export class GroupService {
    private baseUrl: string;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/groups`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    get(id: number) {
       return this.http.get(`${this.baseUrl}/groups/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    add(model : Group) {
        return this.http.post(`${this.baseUrl}/groups`, model, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    edit(model) {
        return this.http.put(`${this.baseUrl}/groups`, model, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    delete(id: number) {
        return this.http.delete(`${this.baseUrl}/groups/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    deactivate(id: number) {
        return this.http.put(`${this.baseUrl}/groups/${id}/active/false`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    activate(id: number) {
        return this.http.put(`${this.baseUrl}/groups/${id}/active/true`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    getOptions() {
        return this.http.get(`${this.baseUrl}/groups/options`, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }
}