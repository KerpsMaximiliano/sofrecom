import { Injectable } from '@angular/core';
import { Service } from '../common/service';
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

  getAllOptions(customerId) {
    return this.http.get<any>(`${this.apiUrl}/${customerId}/options/all`);
  }

  getAllNotRelated(customerId){
    return this.http.get<any>(`${this.apiUrl}/${customerId}/options/notrelated`);
  }

  getOpportunities(serviceId){
    return this.http.get<any>(`${this.apiUrl}/${serviceId}/opportunities`);
  }

  getPurchaseOrders(serviceId){
    return this.http.get<any>(`${this.apiUrl}/${serviceId}/purchaseOrders`);
  }

  getById(customerId, serviceId) {
    return this.http.get<any>(`${this.apiUrl}/${serviceId}/account/${customerId}`);
  }
}
