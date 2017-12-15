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

  getOptions() {
    return this.http.get(`${this.baseUrl}/employees/options`).map((res:Response) => res.json());
  }

  getById(id) {
    return this.http.get(`${this.baseUrl}/employees/${id}`).map((res:Response) => res.json());
  }
}
