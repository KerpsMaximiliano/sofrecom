import { Option } from 'app/models/option';
import { Injectable } from '@angular/core';
import { Response, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";
import { Role } from "app/models/admin/role";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class RoleService {

  private baseUrl: string;

  constructor(
    private http: HttpAuth, 
    private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get(`${this.baseUrl}/roles`).map((res:Response) => res.json());
  }

  getOptions() {
    return this.http.get(`${this.baseUrl}/roles/options`).map((res:Response) => res.json());
  }

  get(id: number) {
    return this.http.get(`${this.baseUrl}/roles/${id}`).map((res:Response) => res.json());
  }

  getDetail(id: number) {
    return this.http.get(`${this.baseUrl}/roles/${id}/detail`).map((res:Response) => res.json());
  }

  add(model : Role) {
    return this.http.post(`${this.baseUrl}/roles`, model).map((res:Response) => res.json());
  }

  edit(model) {
    return this.http.put(`${this.baseUrl}/roles`, model).map((res:Response) => res.json() );
  }

  deactivate(id: number) {
    return this.http.put(`${this.baseUrl}/roles/${id}/active/false`, {}).map((res:Response) => res.json() );
  }

  activate(id: number) {
    return this.http.put(`${this.baseUrl}/roles/${id}/active/true`, {}).map((res:Response) => res.json() );
  }

  assignFunctionalities(roleId: number, functionalities: any){
      return this.http.post(`${this.baseUrl}/roles/${roleId}/functionalities`, functionalities).map((res:Response) => res.json() );
  }

  unAssignFunctionality(roleId: number, functionalityId: number) {
      return this.http.delete(`${this.baseUrl}/roles/${roleId}/functionalities/${functionalityId}`).map((res:Response) => res.json() );
  }

  unAssignFunctionalities(roleId: number, functionalities: any){
      return this.http.put(`${this.baseUrl}/roles/${roleId}/functionalities`, functionalities).map((res:Response) => res.json() );
  }
}
