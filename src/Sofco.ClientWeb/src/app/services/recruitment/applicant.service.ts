import { Injectable } from "@angular/core";
import { Service } from "../common/service";
import { HttpClient } from "@angular/common/http";

@Injectable()
export class ApplicantService {

    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    get(id){
        return this.http.get<any>(`${this.baseUrl}/applicant/${id}`);
    }

    post(json){
        return this.http.post<any>(`${this.baseUrl}/applicant`, json);
    }

    put(id, json){
        return this.http.put<any>(`${this.baseUrl}/applicant/${id}`, json);
    }

    register(id, json){
        return this.http.post<any>(`${this.baseUrl}/applicant/${id}/register`, json);
    }

    changeStatus(id, json){
        return this.http.put<any>(`${this.baseUrl}/applicant/${id}/status`, json);
    }

    search(json){
        return this.http.post<any>(`${this.baseUrl}/applicant/search`, json);
    }

    getHistory(entityId: number) {
        return this.http.get<any>(`${this.baseUrl}/applicant/${entityId}/history`);
    }
}