import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class EmployeeNewsService {

  private apiUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.apiUrl = this.service.UrlApi+"/employees/news";
  }

  getAll() {
    return this.http.get(this.apiUrl).map((res:Response) => res.json());
  }

  delete(id){
    return this.http.delete(`${this.apiUrl}/${id}`).map((res:Response) => res.json());
  }

  add(id){
    return this.http.post(`${this.apiUrl}/${id}`, {}).map((res:Response) => res.json());
  }
}
