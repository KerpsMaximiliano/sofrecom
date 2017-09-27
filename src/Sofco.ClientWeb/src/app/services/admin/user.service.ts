import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";

@Injectable()
export class UserService {
    private baseUrl: string;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/users`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    get(id: number) {
       return this.http.get(`${this.baseUrl}/users/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    getByEmail(mail: string) {
       return this.http.get(`${this.baseUrl}/users/email/${mail}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    getOptions() {
       return this.http.get(`${this.baseUrl}/users/options`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    getDetail(id: string) {
       return this.http.get(`${this.baseUrl}/users/${id}/detail`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
    }

    deactivate(id: number) {
        return this.http.put(`${this.baseUrl}/users/${id}/active/false`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    activate(id: number) {
        return this.http.put(`${this.baseUrl}/users/${id}/active/true`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    assignGroups(userId: number, objToSend: any){
        return this.http.post(`${this.baseUrl}/users/${userId}/groups`, objToSend, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }

    unassignGroup(userId: number, groupId: number) {
        return this.http.delete(`${this.baseUrl}/users/${userId}/group/${groupId}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
    }
}