import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class EmployeeService {

  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get(`${this.baseUrl}/employees`).map((res:Response) => res.json());
  }

  getOptions() {
    return this.http.get(`${this.baseUrl}/employees/options`).map((res:Response) => res.json());
  }

  getById(id) {
    return this.http.get(`${this.baseUrl}/employees/${id}`).map((res:Response) => res.json());
  }

  getProfile(id) {
    return this.http.get(`${this.baseUrl}/employees/${id}/profile`).map((res:Response) => res.json());
  }

  search(model) {
    return this.http.post(`${this.baseUrl}/employees/search`, model).map((res:Response) => res.json());
  }

  sendUnsubscribeNotification(employeeName, json){
    return this.http.post(`${this.baseUrl}/employees/sendUnsubscribeNotification/${employeeName}`, json).map((res:Response) => res.json());
  }
}
