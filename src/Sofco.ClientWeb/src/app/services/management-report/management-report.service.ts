import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Service } from "app/services/common/service";

@Injectable()
export class ManagementReportService {
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getDetail(serviceId) {
    return this.http.get<any>(`${this.baseUrl}/managementReport/${serviceId}`);
  }

  getBilling(serviceId) {
    return this.http.get<any>(`${this.baseUrl}/managementReport/${serviceId}/billing`);
  }

  getCostDetail(serviceId){
    return this.http.get<any>(`${this.baseUrl}/managementReport/${serviceId}/costDetail`);
  }

  PostCostDetail(serviceId, model){
    return this.http.post<any>(`${this.baseUrl}/managementReport/${serviceId}/costDetail`, model);
  }

  PostCostDetailMonth(serviceId, model){
    return this.http.post<any>(`${this.baseUrl}/managementReport/${serviceId}/costDetailMonth`, model);
  }

  getContrated(serviceId, month, year){
    return this.http.get<any>(`${this.baseUrl}/managementReport/${serviceId}/contrated/${month}/${year}`);
  }

}