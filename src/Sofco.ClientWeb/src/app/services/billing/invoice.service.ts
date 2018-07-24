
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Service } from "app/services/common/service";
import { MenuService } from 'app/services/admin/menu.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class InvoiceService {
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service, private menuService: MenuService) {
    this.baseUrl = this.service.UrlApi;
  }

  getUrlForImportFile(id){
    return `${this.baseUrl}/invoices/${id}/file`;
  }
  
  add(model) {
     return this.http.post<any>(`${this.baseUrl}/invoices`, model);
  }

  getById(id) {
     return this.http.get<any>(`${this.baseUrl}/invoices/${id}`);
  }

  delete(id) {
     return this.http.delete<any>(`${this.baseUrl}/invoices/${id}`);
  }

  clone(id) {
    return this.http.post<any>(`${this.baseUrl}/invoices/${id}/clone`, { });
  }

  getOptions(projectId) {
     return this.http.get<any>(`${this.baseUrl}/invoices/${projectId}/options`);
  }

  changeStatus(id, status, comment, invoiceNumber){
    const body = {
      userId: this.menuService.user.id,
      comment: comment,
      status: status,
      invoiceNumber: invoiceNumber
    };

    return this.http.post<any>(`${this.baseUrl}/invoices/${id}/status`, body);
 }

  getPdfFile(id){
    return this.http.get<any>(`${this.baseUrl}/invoices/${id}/pdf`);
  }

  exportPdfFile(id){
    return this.http.get(`${this.baseUrl}/invoices/${id}/pdf/export`, {
      responseType: 'arraybuffer',
      observe: 'response'
    }).pipe(map((res: any) => {
      return new Blob([res.body], { type: 'application/octet-stream' });
    }));
  }

  exportExcelFile(id){
    return this.http.get(`${this.baseUrl}/invoices/${id}/excel/export`, {
      responseType: 'arraybuffer',
      observe: 'response'
    }).pipe(map((res: any) => {
      return new Blob([res.body], { type: 'application/octet-stream' });
    }));
  }

  //todo: borrar
//   getExcel(id) {
//     return this.http.get(`${this.baseUrl}/invoices/${id}/excel`, {
//       responseType: 'arraybuffer',
//       observe: 'response'
//    }).map((res: any) => {
//      return new Blob([res.body], { type: 'application/octet-stream' });
//    });
//   }

//   //todo: borrar
//   downloadPdf(id) {
//     return this.http.get(`${this.baseUrl}/invoices/${id}/pdf/download`, {
//       responseType: 'arraybuffer',
//       observe: 'response'
//     }).map((res: any) => {
//       return new Blob([res.body], { type: 'application/octet-stream' });
//     });
//   }

//  //todo: borrar
//   export(model) {
//     return this.http.post(`${this.baseUrl}/invoices/excel`, model, {
//       responseType: 'arraybuffer',
//       observe: 'response'
//    }).map((res: any) => {
//      return new Blob([res.body], { type: 'application/octet-stream' });
//    });
//   }

  search(parameters) {
    return this.http.post<any>(`${this.baseUrl}/invoices/search`, parameters);
  }

  getStatus() {
    return this.http.get<any>(`${this.baseUrl}/invoices/status`);
  }

  getHistories(invoiceId) {
    return this.http.get<any>(`${this.baseUrl}/invoices/${invoiceId}/histories`);
  }

  askForAnnulment(ids) {
    return this.http.post<any>(`${this.baseUrl}/invoices/requestAnnulment`, ids);
 }
}
