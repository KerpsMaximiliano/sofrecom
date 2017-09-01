import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";

@Injectable()
export class UserService {
    private baseUrl: string;
    private headers: Headers;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
        this.headers = this.service.getHeaders();
    }

    reloadHeaders(){
        this.headers = this.service.getHeaders();
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/user`, { headers: this.headers}).map((res:Response) => res.json());
    }

    get(id: number) {
       return this.http.get(`${this.baseUrl}/user/${id}`, { headers: this.headers}).map((res:Response) => res.json());
    }

    getByEmail(mail: string) {
       return this.http.get(`${this.baseUrl}/user/email/${mail}`, { headers: this.headers}).map((res:Response) => res.json());
    }

    getOptions() {
       return this.http.get(`${this.baseUrl}/user/options`, { headers: this.headers}).map((res:Response) => res.json());
    }

    getDetail(id: string) {
       return this.http.get(`${this.baseUrl}/user/${id}/detail`, { headers: this.headers}).map((res:Response) => res.json());
    }

    deactivate(id: number) {
        return this.http.put(`${this.baseUrl}/user/${id}/active/false`, {}, { headers: this.headers}).map((res:Response) => res.json() );
    }

    activate(id: number) {
        return this.http.put(`${this.baseUrl}/user/${id}/active/true`, {}, { headers: this.headers}).map((res:Response) => res.json() );
    }

    assignGroups(userId: number, objToSend: any){
        return this.http.post(`${this.baseUrl}/user/${userId}/groups`, objToSend, { headers: this.headers}).map((res:Response) => res.json() );
    }

    unassignGroup(userId: number, groupId: number) {
        return this.http.delete(`${this.baseUrl}/user/${userId}/group/${groupId}`, { headers: this.headers}).map((res:Response) => res.json() );
    }
}