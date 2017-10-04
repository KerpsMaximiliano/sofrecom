import { Injectable } from '@angular/core';
import { Response, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { MenuService } from 'app/services/admin/menu.service';
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class InvoiceService {
  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service, private menuService: MenuService) {
    this.baseUrl = this.service.UrlApi;
  }

  getUrlForImportExcel(id){
    return `${this.baseUrl}/invoices/${id}/excel`;
  }

  getUrlForImportPdf(id){
    return `${this.baseUrl}/invoices/${id}/pdf`;
  }
 
  add(model){
     return this.http.post(`${this.baseUrl}/invoices`, model).map((res:Response) => res.json());
  }

  getById(id){
     return this.http.get(`${this.baseUrl}/invoices/${id}`).map((res:Response) => res.json());
  }

  delete(id){
     return this.http.delete(`${this.baseUrl}/invoices/${id}`).map((res:Response) => res.json());
  }

  clone(id){
    return this.http.post(`${this.baseUrl}/invoices/${id}/clone`, { }, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getOptions(projectId){
     return this.http.get(`${this.baseUrl}/invoices/${projectId}/options`).map((res:Response) => res.json());
  }

<<<<<<< HEAD
=======
  annulment(id){
     return this.http.put(`${this.baseUrl}/invoices/${id}/annulment`, {}).map((res:Response) => res.json());
  }

>>>>>>> ca1d0266896eee13b207657eb05fdb73d6d79cbb
  changeStatus(id, status, comment, invoiceNumber){
    var body = {
      userId: this.menuService.user.id,
      comment: comment,
      status: status,
      invoiceNumber: invoiceNumber
    }

    return this.http.post(`${this.baseUrl}/invoices/${id}/status`, body).map((res:Response) => res.json());
 }

  getExcel(id){
     return this.http.get(`${this.baseUrl}/invoices/${id}/excel`,
      {
        responseType: ResponseContentType.Blob
      })
      .map((res:any) => {
        if(res.status >= 300 && res.status <= 500){
          return res.json();
        }
        else{
          return new Blob([res._body],{ type: 'application/vnd.ms-excel' })
        }
      });
  }

  getPdf(id){
     return this.http.get(`${this.baseUrl}/invoices/${id}/pdf`,
      {
        responseType: ResponseContentType.Blob
      })
      .map((res:any) => {
        if(res.status >= 300 && res.status <= 500){
          return res.json();
        }
        else{
          return new Blob([res._body],{ type: 'application/vnd.ms-excel' })
        }
      });
  }

  export(model){
    debugger;
    return this.http.post(`${this.baseUrl}/invoices/excel`,
      model,  
      {
        responseType: ResponseContentType.Blob
      })
      .map((res:any) => new Blob([res._body],{ type: 'application/vnd.ms-excel' }));
  }

  search(parameters) {
    return this.http.post(`${this.baseUrl}/invoices/search`, parameters).map((res:Response) => res.json());
  }

  getStatus() {
    return this.http.get(`${this.baseUrl}/invoices/status`).map((res:Response) => res.json());
  }

  getHistories(invoiceId) {
    return this.http.get(`${this.baseUrl}/invoices/${invoiceId}/histories`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }
}
