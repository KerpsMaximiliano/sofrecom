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

  get(id: number) {
    return this.http.get(`${this.baseUrl}/role/${id}`, { headers: this.headers}).map((res:Response) => res.json());
  }

  add(model : Role) {
    return this.http.post(`${this.baseUrl}/role`, model, { headers: this.headers}).map((res:Response) => res.json());
  }

  edit(model : Role) {
    return this.http.put(`${this.baseUrl}/role`, model, { headers: this.headers}).map((res:Response) => res.json() );
  }

  delete(id: number) {
    return this.http.delete(`${this.baseUrl}/role/${id}`, { headers: this.headers}).map((res:Response) => res.json() );
  }

}
