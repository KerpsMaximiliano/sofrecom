import { Injectable } from '@angular/core';
import { Response, Headers, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class LicenseService {
  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getSectors() {
    return this.http.get(`${this.baseUrl}/utils/sectors`).map((res:Response) => res.json());
  }

  getByStatus(statusId) {
    return this.http.get(`${this.baseUrl}/licenses/status/${statusId}`).map((res:Response) => res.json());
  }

  getByManager(managerId) {
    return this.http.get(`${this.baseUrl}/licenses/manager/${managerId}`).map((res:Response) => res.json());
  }

  getByManagerAndStatus(managerId, statusId) {
    return this.http.get(`${this.baseUrl}/licenses/status/${statusId}/manager/${managerId}`).map((res:Response) => res.json());
  }

  getLicenceTypes() {
    return this.http.get(`${this.baseUrl}/licenses/types`).map((res:Response) => res.json());
  }

  add(model){
    return this.http.post(`${this.baseUrl}/licenses`, model).map((res:Response) => res.json());
  }

  search(params){
    return this.http.post(`${this.baseUrl}/licenses/search`, params).map((res:Response) => res.json());
  }

  getUrlForImportFile(id){
    return `${this.baseUrl}/licenses/${id}/file`;
  }
}
