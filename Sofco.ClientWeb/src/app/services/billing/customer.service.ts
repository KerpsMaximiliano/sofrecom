import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import { Service } from "app/services/common/service";

@Injectable()
export class CustomerService {

  private baseUrl: string;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll(userMail) {
    return this.http.get(`${this.baseUrl}/customer/${userMail}`, { headers: this.service.getHeaders() }).map((res:Response) => res.json());
  }

  getOptions(userMail) {
    return this.http.get(`${this.baseUrl}/customer/${userMail}/options`, { headers: this.service.getHeaders() }).map((res:Response) => res.json());
  }
}
