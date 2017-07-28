import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/service";

@Injectable()
export class FunctionalityService {
    private baseUrl: string;
    private headers: Headers;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
        this.headers = this.service.getHeaders();
    }

    getAll() {
        return this.http.get(`${this.baseUrl}/functionality`, { headers: this.headers}).map((res:Response) => res.json());
    }

    get(id: number) {
       return this.http.get(`${this.baseUrl}/functionality/${id}`, { headers: this.headers}).map((res:Response) => res.json());
    }

    delete(id: number) {
        return this.http.delete(`${this.baseUrl}/functionality/${id}`, { headers: this.headers}).map((res:Response) => res.json() );
    }
}