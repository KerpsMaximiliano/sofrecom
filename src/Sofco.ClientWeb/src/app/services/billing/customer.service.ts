import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from 'app/services/common/service';
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

  getById(customerId) {
    return this.http.get<any>(`${this.apiUrl}/${customerId}`);
  }
}
