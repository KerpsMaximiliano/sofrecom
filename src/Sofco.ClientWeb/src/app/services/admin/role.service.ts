import { Option } from 'app/models/option';
import { Injectable } from '@angular/core';
import { Http, Response, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/common/service";
import { Role } from "app/models/admin/role";

@Injectable()
export class RoleService {

  private baseUrl: string;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get(`${this.baseUrl}/roles`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getOptions() {
    return this.http.get(`${this.baseUrl}/roles/options`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  get(id: number) {
    return this.http.get(`${this.baseUrl}/roles/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getDetail(id: number) {
    return this.http.get(`${this.baseUrl}/roles/${id}/detail`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  add(model : Role) {
    return this.http.post(`${this.baseUrl}/roles`, model, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  edit(model) {
    return this.http.put(`${this.baseUrl}/roles`, model, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
  }

  deactivate(id: number) {
    return this.http.put(`${this.baseUrl}/roles/${id}/active/false`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
  }

  activate(id: number) {
    return this.http.put(`${this.baseUrl}/roles/${id}/active/true`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
  }

  assignFunctionalities(roleId: number, functionalities: any){
      return this.http.post(`${this.baseUrl}/roles/${roleId}/functionalities`, functionalities, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
  }

  unAssignFunctionality(roleId: number, functionalityId: number) {
      return this.http.delete(`${this.baseUrl}/roles/${roleId}/functionalities/${functionalityId}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
  }

  unAssignFunctionalities(roleId: number, functionalities: any){
      return this.http.put(`${this.baseUrl}/roles/${roleId}/functionalities`, functionalities, { headers: this.service.getHeaders()}).map((res:Response) => res.json() );
  }
}
