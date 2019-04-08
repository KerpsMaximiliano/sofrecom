import { Injectable } from '@angular/core';
import { Service } from '../common/service';
import { HttpClient } from '@angular/common/http';
import { UserApproverType } from '../../models/enums/userApproverType';

@Injectable()
export class UserApproverService {
  private apiUrl: string;
  public type: UserApproverType;

  constructor(private http: HttpClient, private service: Service) {
    this.apiUrl = `${this.service.UrlApi}/userApprovers`;
  }

  setType(type:UserApproverType) {
    this.type = type;
  }

  save(model) {
    return this.http.post<any>(this.apiUrl+ `/${this.type}`, model);
  }

  getCurrentEmployees(query) {
    return this.http.get<any>(this.apiUrl + `/${this.type}/employees?analyticId=${query.analyticId}&approvalId=${query.approvalId}`);
  }

  getByUserId(userId) {
    return this.http.get<any>(this.apiUrl + `/${this.type}/${userId}/analytics`);
  }

  getApprovals(analyticId) {
    return this.http.get<any>(this.apiUrl + `/${this.type}/?analyticId=${analyticId}`);
  }

  delete(id: number) {
    return this.http.delete<any>(this.apiUrl + '/' + id);
  }

  deleteAll(ids) {
    return this.http.post<any>(this.apiUrl + '/clean', ids);
  }
}
