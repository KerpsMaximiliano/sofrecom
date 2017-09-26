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
    return `${this.baseUrl}/invoice/${id}/excel`;
  }

  getUrlForImportPdf(id){
    return `${this.baseUrl}/invoice/${id}/pdf`;
  }
 
  add(model){
     return this.http.post(`${this.baseUrl}/invoice`, model, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getById(id){
     return this.http.get(`${this.baseUrl}/invoice/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  delete(id){
     return this.http.delete(`${this.baseUrl}/invoice/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getOptions(projectId){
     return this.http.get(`${this.baseUrl}/invoice/${projectId}/options`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  annulment(id){
     return this.http.put(`${this.baseUrl}/invoice/${id}/annulment`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  changeStatus(id, status, comment, invoiceNumber){
    var body = {
      userId: this.menuService.user.id,
      comment: comment,
      status: status,
      invoiceNumber: invoiceNumber
    }

    return this.http.post(`${this.baseUrl}/invoice/${id}/status`, body, { headers: this.service.getHeaders() }).map((res:Response) => res.json());
 }

  getExcel(id){
     return this.http.get(`${this.baseUrl}/invoice/${id}/excel`,
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
     return this.http.get(`${this.baseUrl}/invoice/${id}/pdf`,
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
    return this.http.post(`${this.baseUrl}/invoice/excel`,
      model,  
      { headers: this.service.getHeaders(),
        responseType: ResponseContentType.Blob
      })
      .map((res:any) => new Blob([res._body],{ type: 'application/vnd.ms-excel' }));
  }

  search(parameters) {
    return this.http.post(`${this.baseUrl}/invoice/search`, parameters, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getStatus() {
    return this.http.get(`${this.baseUrl}/invoice/status`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }
}
