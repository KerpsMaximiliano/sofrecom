import { Injectable } from '@angular/core';
import { Response, Headers } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class ServiceService {

  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll(customerId) {
       return this.http.get(`${this.baseUrl}/services/${customerId}`).map((res:Response) => res.json());
  }

  getOptions(customerId) {  
    return this.http.get(`${this.baseUrl}/services/${customerId}/options`).map((res:Response) => res.json());
  }

  getById(customerId, serviceId) {
    return this.http.get(`${this.baseUrl}/services/${serviceId}/account/${customerId}`).map((res:Response) => res.json()); 
  }

  getPurchaseOrders(serviceId){
    return this.http.get(`${this.baseUrl}/services/${serviceId}/purchaseOrders`).map((res:Response) => res.json());
  }
}
