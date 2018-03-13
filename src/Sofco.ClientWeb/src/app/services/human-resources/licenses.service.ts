import { Injectable } from '@angular/core';
import { Response, Headers, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class LicenseService {
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getSectors() {
    return this.http.get<any>(`${this.baseUrl}/utils/sectors`);
  }

  getByStatus(statusId) {
    return this.http.get<any>(`${this.baseUrl}/licenses/status/${statusId}`);
  }

  getByManager(managerId) {
    return this.http.get<any>(`${this.baseUrl}/licenses/manager/${managerId}`);
  }

  getByManagerAndStatus(managerId, statusId) {
    return this.http.get<any>(`${this.baseUrl}/licenses/status/${statusId}/manager/${managerId}`);
  }

  getLicenceTypes() {
    return this.http.get<any>(`${this.baseUrl}/licenses/types`);
  }

  add(model){
    return this.http.post<any>(`${this.baseUrl}/licenses`, model);
  }

  search(params){
    return this.http.post<any>(`${this.baseUrl}/licenses/search`, params);
  }

  getUrlForImportFile(id){
    return `${this.baseUrl}/licenses/${id}/file`;
  }
}
