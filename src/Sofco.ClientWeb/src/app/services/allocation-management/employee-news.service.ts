import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class EmployeeNewsService {

  private apiUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.apiUrl = this.service.UrlApi + "/employees/news";
  }

  getAll() {
    return this.http.get<any>(this.apiUrl);
  }

  delete(id){
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  add(id){
    return this.http.post<any>(`${this.apiUrl}/${id}`, {});
  }

  cancel(id){
    return this.http.post<any>(`${this.apiUrl}/cancel/${id}`, {});
  }
}
