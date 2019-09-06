import { Injectable } from "@angular/core";
import { Service } from "../common/service";
import { HttpClient } from "@angular/common/http";

@Injectable()
export class JobSearchService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getApplicants(){
        return this.http.get<any>(`${this.baseUrl}/jobSearch/applicants`);
    }

    getRecruiters(){
        return this.http.get<any>(`${this.baseUrl}/jobSearch/recruiters`);
    }

    post(json){
        return this.http.post<any>(`${this.baseUrl}/jobSearch`, json);
    }
}