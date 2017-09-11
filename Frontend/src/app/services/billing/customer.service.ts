import { Injectable } from '@angular/core';
import { Http, Response, Headers } from '@angular/http';
import { Service } from "app/services/common/service";

@Injectable()
export class CustomerService {

  private baseUrl: string;
  private headers: Headers;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlApi;
    this.headers = this.service.getHeaders();
  }

  getAll(userMail) {
    return this.http.get(`${this.baseUrl}/customer/${userMail}`, { headers: this.headers }).map((res:Response) => res.json());
  }

  getOptions(userMail) {
    return this.http.get(`${this.baseUrl}/customer/${userMail}/options`, { headers: this.headers }).map((res:Response) => res.json());
  }
}
