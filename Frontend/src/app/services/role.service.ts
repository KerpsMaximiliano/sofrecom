import { Injectable } from '@angular/core';
import { Role } from "models/role";
import { Http, Response, Headers, URLSearchParams } from '@angular/http';
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

  get(id: string) {
    return this.http.get(`${this.baseUrl}/role/${id}`, { headers: this.headers}).map((res:Response) => res.json());
  }

  save(model : Role) {
    return this.http.post(`${this.baseUrl}/role`, model, { headers: this.headers}).map((res:Response) => res );
  }

}
