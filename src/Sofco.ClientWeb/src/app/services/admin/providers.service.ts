import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Service } from "../common/service";

@Injectable({
    providedIn: 'root'
})

export class ProvidersService {

    private mode: string;
    private baseUrl: string;

    constructor(
        private http: HttpClient,
        private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    public getAll(): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/providers`);
    }

    public getByParams(params: {statusId: number, businessName: string, providersArea: Array<number>}): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/providers/GetByParams`, params);
    }

    public get(id: number): Observable<any> {
        return this.http.get<any>(`${this.baseUrl}/providers/${id}`);
    }

    public edit(id: number, provider: any) {
        return this.http.put<any>(`${this.baseUrl}/providers/${id}`, provider, {observe: 'response'});
    }

    public post(provider: any): Observable<any> {
        return this.http.post<any>(`${this.baseUrl}/providers`, provider, {observe: 'response'});
    }

    // public getMode() {
    //     return this.mode;
    // }

    public setMode(mode: string) {
        this.mode = mode;
    }
}