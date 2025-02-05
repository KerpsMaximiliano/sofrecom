import { Injectable } from '@angular/core';
import { Service } from "./service";
import { HttpClient } from '@angular/common/http';
import { MenuService } from '../admin/menu.service';

@Injectable()
export class UtilsService {

  private apiUrl: string;

  constructor(private http: HttpClient, private service: Service, private menuService: MenuService) {
    this.apiUrl = this.service.UrlApi + '/utils';
  }

  getMonths() {
    return this.http.get<any>(`${this.apiUrl}/months`);
  }

  getPrepaids() {
    return this.http.get<any>(`${this.apiUrl}/prepaids`);
  }

  getCloseMonths() {
    return this.http.get<any>(`${this.apiUrl}/closeMonths`);
  }

  getYears() {
    return this.http.get<any>(`${this.apiUrl}/years`);
  }

  getCurrencies() {
    return this.http.get<any>(`${this.apiUrl}/currencies`);
  }

  getAreas() {
    return this.http.get<any>(`${this.apiUrl}/areas`);
  }

  getSectors() {
    return this.http.get<any>(`${this.apiUrl}/sectors`);
  }

  getUserDelegateTypes() {
    return this.http.get<any>(`${this.apiUrl}/userDelegateTypes`);
  }

  getMonthsReturn(): any {
    return this.http.get<any>(`${this.apiUrl}/monthsReturn`);
  }

  getCreditCards() {
    return this.http.get<any>(`${this.apiUrl}/creditCards`);
  }

  getBanks() {
    return this.http.get<any>(`${this.apiUrl}/Banks`);
  }

  getEmployeeProfiles(){
    return this.http.get<any>(`${this.apiUrl}/EmployeeProfile`)
  }
}