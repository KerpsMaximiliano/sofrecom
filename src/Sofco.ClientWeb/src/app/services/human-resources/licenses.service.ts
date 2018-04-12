import { Injectable } from '@angular/core';
import { Response, Headers, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';
import { MenuService } from '../admin/menu.service';

@Injectable()
export class LicenseService {
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service, private menuService: MenuService) {
    this.baseUrl = this.service.UrlApi;
  }

  getById(id) {
    return this.http.get<any>(`${this.baseUrl}/licenses/${id}`);
  }

  getHistories(id){
    return this.http.get<any>(`${this.baseUrl}/licenses/${id}/histories`);
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

  getByEmployee(employeeId) {
    return this.http.get<any>(`${this.baseUrl}/licenses/employee/${employeeId}`);
  }

  getByManagerAndStatus(managerId, statusId) {
    return this.http.get<any>(`${this.baseUrl}/licenses/status/${statusId}/manager/${managerId}`);
  }

  getLicenceTypes() {
    return this.http.get<any>(`${this.baseUrl}/licenses/types`);
  }

  add(model){
    model.userId = this.menuService.user.id;
    model.isRrhh = this.menuService.userIsRrhh;
    model.employeeLoggedId = this.menuService.user.employeeId;
    return this.http.post<any>(`${this.baseUrl}/licenses`, model);
  }

  changeStatus(id, json){
    json.userId = this.menuService.user.id;
    json.isRrhh = this.menuService.userIsRrhh;
    return this.http.post<any>(`${this.baseUrl}/licenses/${id}/status`, json);
  }

  search(params){
    return this.http.post<any>(`${this.baseUrl}/licenses/search`, params);
  }

  fileDelivered(id){
    return this.http.put<any>(`${this.baseUrl}/licenses/${id}/fileDelivered`, {});
  }

  getUrlForImportFile(id){
    return `${this.baseUrl}/licenses/${id}/file`;
  }

  deleteFile(id){
    return this.http.delete<any>(`${this.baseUrl}/licenses/file/${id}`);
  }

  createReport() {
    return this.http.get(`${this.baseUrl}/licenses/report`,
     {
       responseType: 'blob'
     });
  }
}
