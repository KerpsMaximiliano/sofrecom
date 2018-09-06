import { Injectable } from '@angular/core';
import { Service } from '../common/service';
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

  getCurrentEmployees(query) {
    return this.http.get<any>(this.apiUrl + `/employees?analyticId=${query.analyticId}&approvalId=${query.approvalId}`);
  }

  getApprovals(analyticId) {
    return this.http.get<any>(this.apiUrl + `/approvers?analyticId=${analyticId}`);
  }

  delete(id: number) {
    return this.http.delete<any>(this.apiUrl + '/' + id);
  }

  deleteAll(ids) {
    return this.http.post<any>(this.apiUrl + '/clean', ids);
  }
}
