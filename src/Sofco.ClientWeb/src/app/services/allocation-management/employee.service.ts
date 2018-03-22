import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class EmployeeService {

  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get<any>(`${this.baseUrl}/employees`);
  }

  getManagers() {
    return this.http.get<any>(`${this.baseUrl}/users/managers`);
  }

  getOptions() {
    return this.http.get<any>(`${this.baseUrl}/employees/options`);
  }

  getById(id) {
    return this.http.get<any>(`${this.baseUrl}/employees/${id}`);
  }

  getProfile(id) {
    return this.http.get<any>(`${this.baseUrl}/employees/${id}/profile`);
  }

  search(model) {
    return this.http.post<any>(`${this.baseUrl}/employees/search`, model);
  }

  sendUnsubscribeNotification(employeeName, json){
    return this.http.post<any>(`${this.baseUrl}/employees/sendUnsubscribeNotification/${employeeName}`, json);
  }

  finalizeExtraHolidays(id){
    return this.http.put<any>(`${this.baseUrl}/employees/${id}/finalizeExtraHolidays`, {});
  }
}
