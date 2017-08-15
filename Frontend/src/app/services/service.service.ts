import { Option } from 'models/option';
import { Injectable } from '@angular/core';
import { Role } from "models/role";
import { Http, Response, Headers, URLSearchParams, RequestOptions } from '@angular/http';
import { Service } from "app/services/service";

@Injectable()
export class ServiceService {

  private baseUrl: string;
  private headers: Headers;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlApiNode;
    this.headers = this.service.getHeaders();
  }

  getAll(custId: number) {
    return this.http.get(`${this.baseUrl}/solfac/customer/${custId}/service`, { headers: this.headers}).map((res:Response) => res.json());
  }

  get(id: number) {
    return this.http.get(`${this.baseUrl}/solfac/service/${id}`, { headers: this.headers}).map((res:Response) => res.json());
  }

  /*getDetail(id: number) {
    return this.http.get(`${this.baseUrl}/customer/${id}/detail`, { headers: this.headers}).map((res:Response) => res.json());
  }

  add(model : Role) {
    return this.http.post(`${this.baseUrl}/customer`, model, { headers: this.headers}).map((res:Response) => res.json());
  }

  edit(model) {
    return this.http.put(`${this.baseUrl}/customer`, model, { headers: this.headers}).map((res:Response) => res.json() );
  }

  delete(id: number) {
    return this.http.delete(`${this.baseUrl}/customer/${id}`, { headers: this.headers}).map((res:Response) => res.json() );
  }*/

  deactivate(id: number) {
    return this.http.put(`${this.baseUrl}/solfac/service/${id}/active/false`, { headers: this.headers}).map((res:Response) => res.json() );
  }

  activate(id: number) {
    return this.http.put(`${this.baseUrl}/solfac/service/${id}/active/true`, { headers: this.headers}).map((res:Response) => res.json() );
  }

}
