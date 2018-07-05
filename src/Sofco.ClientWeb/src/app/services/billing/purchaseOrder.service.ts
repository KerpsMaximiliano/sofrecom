import { Injectable } from '@angular/core';
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

  getPendings() {
    return this.http.get<any>(`${this.baseUrl}/purchaseOrders/pendings`);
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

  makeAdjustment(id, model) {
    return this.http.put<any>(`${this.baseUrl}/purchaseOrders/${id}/adjustment`, model);
  }

  getUrlForImportExcel(id){
    return `${this.baseUrl}/purchaseOrders/${id}/file`;
  }

  deleteFile(id) {
    return this.http.delete<any>(`${this.baseUrl}/purchaseOrders/${id}/file`);
  }

  getFile(id){
    return this.http.get<any>(`${this.baseUrl}/purchaseOrders/${id}/file`);
  }

  exportFile(id){
    return this.http.get(`${this.baseUrl}/purchaseOrders/export/${id}`, {
      responseType: 'arraybuffer',
      observe: 'response'
    }).map((res: any) => {
      return new Blob([res.body], { type: 'application/octet-stream' });
    });
  }

  getReport(params) {
    return this.http.post<any>(`${this.baseUrl}/reports/purchaseOrders`, params);
  }

  getAnalyticsByCurrentUser() {
    return this.http.get<any>(`${this.baseUrl}/reports/purchaseOrders/analytics/options`);
  }

  changeStatus(id, json) {
    return this.http.post<any>(`${this.baseUrl}/purchaseOrders/${id}/status`, json);
  }

  close(id, json) {
    return this.http.post<any>(`${this.baseUrl}/purchaseOrders/${id}/close`, json);
  }

  getHistories(id){
    return this.http.get<any>(`${this.baseUrl}/purchaseOrders/${id}/histories`);
  }
}

