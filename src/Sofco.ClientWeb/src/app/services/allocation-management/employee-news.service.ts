import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class EmployeeNewsService {

  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get(`${this.baseUrl}/employeenews`).map((res:Response) => res.json());
  }

  delete(id){
    return this.http.delete(`${this.baseUrl}/employeenews/${id}`).map((res:Response) => res.json());
  }

  add(id){
    return this.http.post(`${this.baseUrl}/employeenews/${id}`, {}).map((res:Response) => res.json());
  }
}
