import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Service } from "../common/service";

@Injectable({
    providedIn: 'root'
})

export class ProvidersAreaService {

    private baseUrl: string;

    constructor(
        private http: HttpClient,
        private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    public getAll(): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/providersArea`);
    }

    public get(id: number): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/providersArea/${id}`);
    }

    public edit(id: number, providerArea: any) {
        return this.http.put<any>(`${this.baseUrl}/providersArea/${id}`, providerArea);
    }

    public post(providerArea: any) {
        return this.http.post<any>(`${this.baseUrl}/providersArea`, providerArea);
    }

}