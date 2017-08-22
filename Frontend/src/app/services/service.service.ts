import { Injectable } from '@angular/core';
import { Role } from "models/role";
import { Http, Response, Headers } from '@angular/http';
import { Service } from "app/services/service";

@Injectable()
export class ServiceService {

  private baseUrl: string;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlCRM;
  }

  getAll(customerId) {
       return this.http.get(`${this.baseUrl}/service?idAccount=${customerId}`).map((res:Response) => res.json());
  }
}
