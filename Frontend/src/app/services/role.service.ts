import { Option } from 'models/option';
import { Injectable } from '@angular/core';
import { Role } from "models/role";
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/service";

@Injectable()
export class RoleService {

  private baseUrl: string;
  private headers: Headers;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlApi;
    this.headers = this.service.getHeaders();
  }

  getAll() {
    return this.http.get(`${this.baseUrl}/role`, { headers: this.headers}).map((res:Response) => res.json());
  }

  getOptions() {
    return this.http.get(`${this.baseUrl}/role/options`, { headers: this.headers}).map((res:Response) => res.json());
  }

  get(id: number) {
    return this.http.get(`${this.baseUrl}/role/${id}`, { headers: this.headers}).map((res:Response) => res.json());
  }

  getDetail(id: number) {
    return this.http.get(`${this.baseUrl}/role/${id}/detail`, { headers: this.headers}).map((res:Response) => res.json());
  }

  add(model : Role) {
    return this.http.post(`${this.baseUrl}/role`, model, { headers: this.headers}).map((res:Response) => res.json());
  }

  edit(model) {
    return this.http.put(`${this.baseUrl}/role`, model, { headers: this.headers}).map((res:Response) => res.json() );
  }

  delete(id: number) {
    return this.http.delete(`${this.baseUrl}/role/${id}`, { headers: this.headers}).map((res:Response) => res.json() );
  }

  deactivate(id: number) {
    return this.http.put(`${this.baseUrl}/role/${id}/active/false`, { headers: this.headers}).map((res:Response) => res.json() );
  }

  activate(id: number) {
    return this.http.put(`${this.baseUrl}/role/${id}/active/true`, { headers: this.headers}).map((res:Response) => res.json() );
  }

  assignModules(roleId: number, modules: any){
      return this.http.post(`${this.baseUrl}/role/${roleId}/modules`, modules, { headers: this.headers}).map((res:Response) => res.json() );
  }

  unAssignModule(roleId: number, moduleId: number) {
      return this.http.delete(`${this.baseUrl}/role/${roleId}/modules/${moduleId}`, { headers: this.headers}).map((res:Response) => res.json() );
  }
}
