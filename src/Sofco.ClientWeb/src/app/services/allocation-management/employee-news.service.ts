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

  delete(id, json){
    return this.http.put<any>(`${this.apiUrl}/${id}`, json);
  }

  add(id){
    return this.http.post<any>(`${this.apiUrl}/${id}`, {});
  }

  cancel(id){
    return this.http.post<any>(`${this.apiUrl}/cancel/${id}`, {});
  }

  getTypeEndReasons() {
    return this.http.get<any>(`${this.service.UrlApi}/utils/employeeTypeEndReasons`);
  }

  update(){
    return this.http.post<any>(`${this.apiUrl}/update/`, {});
  }
}
