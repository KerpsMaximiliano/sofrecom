import { Injectable } from '@angular/core';
import { Response, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class PurchaseOrderService {

  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get<any>(`${this.baseUrl}/purchaseOrders`);
  }

  getStatuses() {
    return this.http.get<any>(`${this.baseUrl}/purchaseOrders/status`);
  }

  getFormOptions() {
    return this.http.get<any>(`${this.baseUrl}/purchaseOrders/formOptions`);
  }

  getById(id) {
    return this.http.get<any>(`${this.baseUrl}/purchaseOrders/${id}`);
  }

  add(model) {
    return this.http.post<any>(`${this.baseUrl}/purchaseOrders`, model);
  }

  search(params) {
    return this.http.post<any>(`${this.baseUrl}/purchaseOrders/search`, params);
  }

  update(model) {
    return this.http.put<any>(`${this.baseUrl}/purchaseOrders`, model);
  }

  getUrlForImportExcel(id){
    return `${this.baseUrl}/purchaseOrders/${id}/file`;
  }

  deleteFile(id) {
    return this.http.delete<any>(`${this.baseUrl}/purchaseOrders/${id}/file`);
  }

  exportFile(id){
    return this.http.get(`${this.baseUrl}/purchaseOrders/export/${id}`,
     {
       responseType: 'blob'
     })
     .map((res: any) => {
       if (res.status >= 300 && res.status <= 500){
         return res.json();
       } else {
         return new Blob([res._body], { type: 'application/octet-stream' });
       }
     });
 }
}

