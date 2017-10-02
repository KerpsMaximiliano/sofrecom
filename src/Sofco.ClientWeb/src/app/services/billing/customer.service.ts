import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class CustomerService {

  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll(userMail) {
    return this.http.get(`${this.baseUrl}/customers/${userMail}`).map((res:Response) => res.json());
  }

  getOptions(userMail) {
    return this.http.get(`${this.baseUrl}/customers/${userMail}/options`).map((res:Response) => res.json());
  }

  getById(customerId) {
    return this.http.get(`${this.baseUrl}/customers/${customerId}`).map((res:Response) => res.json()); 
  }
}
