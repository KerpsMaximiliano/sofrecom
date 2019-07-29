import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Service } from "app/services/common/service";

@Injectable()
export class ManagementReportStaffService {
  
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getDetail(id) {
    return this.http.get<any>(`${this.baseUrl}/managementReportStaff/${id}`);
  }

  send(model){
    return this.http.put<any>(`${this.baseUrl}/managementReportStaff/send`, model);
  }

  close(model){
    return this.http.put<any>(`${this.baseUrl}/managementReportStaff/close`, model);
  }

  updateDates(id, model){
    return this.http.put<any>(`${this.baseUrl}/managementReport/${id}/dates`, model);
  }

  addBudget(id, model){
    return this.http.post<any>(`${this.baseUrl}/managementReportStaff/${id}/budget`, model);
  }

  updateBudget(id, model){
    return this.http.put<any>(`${this.baseUrl}/managementReportStaff/${id}/budget`, model);
  }

  deleteBudget(id, budgetId){
    return this.http.delete<any>(`${this.baseUrl}/managementReportStaff/${id}/budget/${budgetId}`);
  }

  getCostDetailMonth(id, month, year){
    return this.http.get<any>(`${this.baseUrl}/managementReportStaff/${id}/costDetailMonth/${month}/${year}`);
  }

  getCostDetailStaff(serviceId){
    return this.http.get<any>(`${this.baseUrl}/managementReportStaff/${serviceId}/costDetailStaff`);
  }

  PostCostDetailStaff(model){
    return this.http.post<any>(`${this.baseUrl}/managementReportStaff/costDetailStaff`, model);
  }
}