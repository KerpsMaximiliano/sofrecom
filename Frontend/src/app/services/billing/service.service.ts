import { Injectable } from '@angular/core';
import { Role } from "models/role";
import { Http, Response, Headers } from '@angular/http';
import { Service } from "app/services/common/service";

@Injectable()
export class ServiceService {

  private baseUrl: string;
  private headers: Headers;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlApi;
    this.headers = this.service.getHeaders();
  }

  getAll(customerId) {
       return this.http.get(`${this.baseUrl}/service/${customerId}`, { headers: this.headers }).map((res:Response) => res.json());
  }
}
