import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/service";

@Injectable()
export class UserService {
    private baseUrl: string;
    private headers: Headers;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
        this.headers = this.service.getHeaders();
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/user`, { headers: this.headers}).map((res:Response) => res.json());
    }

    get(id: string) {
       return this.http.get(`${this.baseUrl}/user/${id}`, { headers: this.headers}).map((res:Response) => res.json());
    }

    getDetail(id: string) {
       return this.http.get(`${this.baseUrl}/user/${id}/detail`, { headers: this.headers}).map((res:Response) => res.json());
    }
}