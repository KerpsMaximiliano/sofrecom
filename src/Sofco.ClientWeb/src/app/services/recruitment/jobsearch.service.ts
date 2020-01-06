import { Injectable } from "@angular/core";
import { Service } from "../common/service";
import { HttpClient } from "@angular/common/http";

@Injectable()
export class JobSearchService {
    private baseUrl: string;

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;
    }

    getUrlForImportExcel(applicantId, jobSearchId, date){
        return `${this.baseUrl}/jobSearchApplicant/${applicantId}/${jobSearchId}/${date}/file`;
    }

    get(id){
        return this.http.get<any>(`${this.baseUrl}/jobSearch/${id}`);
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

    put(id, json){
        return this.http.put<any>(`${this.baseUrl}/jobSearch/${id}`, json);
    }

    changeStatus(id, json){
        return this.http.put<any>(`${this.baseUrl}/jobSearch/${id}/status`, json);
    }

    search(json){
        return this.http.post<any>(`${this.baseUrl}/jobSearch/search`, json);
    }
    
    getApplicantsRelated(jobSearchId, applicantId){
        return this.http.get<any>(`${this.baseUrl}/jobSearchApplicant?jobSearchId=${jobSearchId}&applicantId=${applicantId}`);
    }

    addContacts(json) {
        return this.http.post<any>(`${this.baseUrl}/jobSearchApplicant`, json);
    }

    addInterview(json, applicantId, jobSearchId, date){
        return this.http.post<any>(`${this.baseUrl}/jobSearchApplicant/${applicantId}/${jobSearchId}/${date}/interview`, json);
    }

    getHistories(id) {
        return this.http.get<any>(`${this.baseUrl}/jobSearch/${id}/history`);
    }
}