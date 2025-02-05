import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class PurchaseOrderApprovalDelegateService {

  private apiUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.apiUrl = `${this.service.UrlApi}/purchaseOrders/approvals/delegates`;
  }

  save(model) {
    return this.http.post<any>(this.apiUrl, model);
  }

  getAll() {
    return this.http.get<any>(this.apiUrl);
  }

  delete(id: number) {
    return this.http.delete<any>(this.apiUrl + '/' + id);
  }

  getAreas() {
    return this.http.get<any>(this.apiUrl + '/areas');
  }

  getSectors() {
    return this.http.get<any>(this.apiUrl + '/sectors');
  }

  getCompliances() {
    return this.http.get<any>(this.apiUrl + '/compliances');
  }

  getDafs() {
    return this.http.get<any>(this.apiUrl + '/dafs');
  }
}

