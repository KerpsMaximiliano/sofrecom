import { Injectable } from '@angular/core';
import { Http, Response, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { MenuService } from 'app/services/admin/menu.service';

@Injectable()
export class InvoiceService {
  private baseUrl: string;

  constructor(private http: Http, private service: Service, private menuService: MenuService) {
    this.baseUrl = this.service.UrlApi;
  }

  getUrlForImportExcel(id){
    return `${this.baseUrl}/invoices/${id}/excel`;
  }

  getUrlForImportPdf(id){
    return `${this.baseUrl}/invoices/${id}/pdf`;
  }
 
  add(model){
     return this.http.post(`${this.baseUrl}/invoices`, model, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getById(id){
     return this.http.get(`${this.baseUrl}/invoices/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  delete(id){
     return this.http.delete(`${this.baseUrl}/invoices/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  clone(id){
    return this.http.post(`${this.baseUrl}/invoices/${id}/clone`, { }, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getOptions(projectId){
     return this.http.get(`${this.baseUrl}/invoices/${projectId}/options`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  changeStatus(id, status, comment, invoiceNumber){
    var body = {
      userId: this.menuService.user.id,
      comment: comment,
      status: status,
      invoiceNumber: invoiceNumber
    }

    return this.http.post(`${this.baseUrl}/invoices/${id}/status`, body, { headers: this.service.getHeaders() }).map((res:Response) => res.json());
 }

  getExcel(id){
     return this.http.get(`${this.baseUrl}/invoices/${id}/excel`,
      { headers: this.service.getHeaders(),
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
      { headers: this.service.getHeaders(),
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
    return this.http.post(`${this.baseUrl}/invoices/excel`,
      model,  
      { headers: this.service.getHeaders(),
        responseType: ResponseContentType.Blob
      })
      .map((res:any) => new Blob([res._body],{ type: 'application/vnd.ms-excel' }));
  }

  search(parameters) {
    return this.http.post(`${this.baseUrl}/invoices/search`, parameters, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getStatus() {
    return this.http.get(`${this.baseUrl}/invoices/status`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getHistories(invoiceId) {
    return this.http.get(`${this.baseUrl}/invoices/${invoiceId}/histories`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }
}
