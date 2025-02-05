import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class EmployeeProfileHistoryService {

  private apiUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.apiUrl = this.service.UrlApi + '/employeeProfileHistories';
  }

  getByEmployeeNumber(employeeNumber) {
    return this.http.get<any>(`${this.apiUrl}/${employeeNumber}`);
  }
}
