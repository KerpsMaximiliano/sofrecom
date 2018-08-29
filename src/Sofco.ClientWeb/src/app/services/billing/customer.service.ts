import { Injectable } from '@angular/core';
import { Service } from '../common/service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class CustomerService {

  private apiUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.apiUrl = this.service.UrlApi + '/customers';
  }

  getAll() {
    return this.http.get<any>(this.apiUrl);
  }

  getOptions() {
    return this.http.get<any>(`${this.apiUrl}/options`);
  }

  getAllOptions() {
    return this.http.get<any>(`${this.apiUrl}/all/options`);
  }

  getOptionsByCurrentManager() {
    return this.http.get<any>(`${this.apiUrl}/options/currentManager`);
  }

  getById(customerId) {
    return this.http.get<any>(`${this.apiUrl}/${customerId}`);
  }

  update() {
    return this.http.put<any>(this.apiUrl, {});
  }
}
