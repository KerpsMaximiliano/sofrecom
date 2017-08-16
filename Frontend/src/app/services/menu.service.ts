import { Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/service";

@Injectable()
export class MenuService {
    private baseUrl: string;
    private headers: Headers;

    constructor(private http: Http, private service: Service) {
        this.baseUrl = this.service.UrlApi;
        this.headers = this.service.getHeaders();
    }

    get(userName: string) {
       return this.http.get(`${this.baseUrl}/menu/${userName}`, { headers: this.headers}).map((res:Response) => res.json());
    }

}