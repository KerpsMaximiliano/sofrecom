import { Injectable } from '@angular/core';
import { Http, Response, Headers } from '@angular/http';
import { Service } from "app/services/common/service";

@Injectable()
export class ServiceService {

  private baseUrl: string;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll(customerId) {
       return this.http.get(`${this.baseUrl}/services/${customerId}`, { headers: this.service.getHeaders() }).map((res:Response) => res.json());
  }

  getOptions(customerId) {  
    return this.http.get(`${this.baseUrl}/services/${customerId}/options`, { headers: this.service.getHeaders() }).map((res:Response) => res.json());
  }
}
