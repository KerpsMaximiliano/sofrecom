import { Injectable } from '@angular/core';
import { Response, Headers, ResponseContentType } from '@angular/http';
import { Service } from 'app/services/common/service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class WorkTimeApprovalDelegateService {
  private apiUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.apiUrl = `${this.service.UrlApi}/workTimeApprovals`;
  }

  save(model) {
    return this.http.post<any>(this.apiUrl, model);
  }

  getAll() {
    return this.http.get<any>(this.apiUrl);
  }

  getCurrentEmployees(query) {
    return this.http.get<any>(this.apiUrl + `/employees?customerId=${query.customerId}&serviceId=${query.serviceId}&approvalId=${query.approvalId}`);
  }

  getApprovals(query) {
    return this.http.get<any>(this.apiUrl + `/approvers?customerId=${query.customerId}&serviceId=${query.serviceId}`);
  }

  delete(id: number) {
    return this.http.delete<any>(this.apiUrl + '/' + id);
  }
}
