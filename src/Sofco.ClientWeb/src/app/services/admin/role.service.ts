import { HttpClient } from '@angular/common/http';
import { Option } from 'app/models/option';
import { Injectable } from '@angular/core';
import { Response, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from 'app/services/common/service';
import { Role } from 'app/models/admin/role';

@Injectable()
export class RoleService {

  private baseUrl: string;

  constructor(
    private http: HttpClient,
    private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get<any>(`${this.baseUrl}/roles`);
  }

  getOptions() {
    return this.http.get<any>(`${this.baseUrl}/roles/options`);
  }

  get(id: number) {
    return this.http.get<any>(`${this.baseUrl}/roles/${id}`);
  }

  getDetail(id: number) {
    return this.http.get<any>(`${this.baseUrl}/roles/${id}/detail`);
  }

  add(model: Role) {
    return this.http.post<any>(`${this.baseUrl}/roles`, model);
  }

  edit(model) {
    return this.http.put<any>(`${this.baseUrl}/roles`, model);
  }

  deactivate(id: number) {
    return this.http.put<any>(`${this.baseUrl}/roles/${id}/active/false`, {});
  }

  activate(id: number) {
    return this.http.put<any>(`${this.baseUrl}/roles/${id}/active/true`, {});
  }

  assignFunctionalities(roleId: number, functionalities: any) {
      return this.http.post<any>(`${this.baseUrl}/roles/${roleId}/functionalities`, functionalities);
  }

  unAssignFunctionality(roleId: number, functionalityId: number) {
      return this.http.delete<any>(`${this.baseUrl}/roles/${roleId}/functionalities/${functionalityId}`);
  }

  unAssignFunctionalities(roleId: number, functionalities: any) {
      return this.http.put<any>(`${this.baseUrl}/roles/${roleId}/functionalities`, functionalities);
  }
}
