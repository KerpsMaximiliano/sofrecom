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

  getById(id) {
    return this.http.get(`${this.baseUrl}/employees/${id}`).map((res:Response) => res.json());
  }

  search(model) {
    return this.http.post(`${this.baseUrl}/employees/search`, model).map((res:Response) => res.json());
  }

  getNews(){
    return this.http.get(`${this.baseUrl}/employees/news`).map((res:Response) => res.json());
  }

  deleteNews(id){
    return this.http.delete(`${this.baseUrl}/employees/news/${id}`).map((res:Response) => res.json());
  }

  add(id){
    return this.http.post(`${this.baseUrl}/employees/${id}`, {}).map((res:Response) => res.json());
  }

  delete(id){
    return this.http.delete(`${this.baseUrl}/employees/${id}`).map((res:Response) => res.json());
  }
}
