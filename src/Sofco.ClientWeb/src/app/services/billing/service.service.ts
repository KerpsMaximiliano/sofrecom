import { Injectable } from '@angular/core';
import { Response, Headers } from '@angular/http';
import { Service } from 'app/services/common/service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ServiceService {

  private apiUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.apiUrl = this.service.UrlApi + '/services';
  }

  getAll(customerId) {
       return this.http.get<any>(`${this.apiUrl}/${customerId}`);
  }

  getOptions(customerId) {
    return this.http.get<any>(`${this.apiUrl}/${customerId}/options`);
  }

  getById(customerId, serviceId) {
    return this.http.get<any>(`${this.apiUrl}/${serviceId}/account/${customerId}`);
  }

  getPurchaseOrders(serviceId) {
    return this.http.get<any>(`${this.apiUrl}/${serviceId}/purchaseOrders`);
  }
}
