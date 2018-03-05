import { Injectable } from '@angular/core';
import { Response, Headers } from '@angular/http';
import { Service } from 'app/services/common/service';
import { HttpAuth } from 'app/services/common/http-auth';

@Injectable()
export class ServiceService {

  private apiUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.apiUrl = this.service.UrlApi + '/services';
  }

  getAll(customerId) {
       return this.http.get(`${this.apiUrl}/${customerId}`).map((res: Response) => res.json().data);
  }

  getOptions(customerId) {
    return this.http.get(`${this.apiUrl}/${customerId}/options`).map((res: Response) => res.json().data);
  }

  getById(customerId, serviceId) {
    return this.http.get(`${this.apiUrl}/${serviceId}/account/${customerId}`).map((res: Response) => res.json().data);
  }

  getPurchaseOrders(serviceId) {
    return this.http.get(`${this.apiUrl}/${serviceId}/purchaseOrders`).map((res: Response) => res.json());
  }
}
