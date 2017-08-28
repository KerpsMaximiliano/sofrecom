import { Injectable } from '@angular/core';
import { Role } from "models/role";
import { Http, Response, Headers } from '@angular/http';
import { Service } from "app/services/service";

@Injectable()
export class CustomerService {

  private baseUrl: string;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlCRM;
  }

  getAll(userMail) {
    return this.http.get(`${this.baseUrl}/account?idManager=${userMail}`).map((res:Response) => res.json());
  }
}
