import { Injectable } from '@angular/core';
import { Http, Response, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";

@Injectable()
export class InvoiceService {
  private baseUrl: string;

  constructor(private http: Http, private service: Service) {
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

  sendToDaf(id){
     return this.http.put(`${this.baseUrl}/invoice/${id}/sendToDaf`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  reject(id){
     return this.http.put(`${this.baseUrl}/invoice/${id}/reject`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  annulment(id){
     return this.http.put(`${this.baseUrl}/invoice/${id}/annulment`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  approve(id, invoiceNumber){
     return this.http.put(`${this.baseUrl}/invoice/${id}/approve/${invoiceNumber}`, {}, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getExcel(id){
     return this.http.get(`${this.baseUrl}/invoice/${id}/excel`,
      { headers: this.service.getHeaders(),
        responseType: ResponseContentType.Blob
      })
      .map((res) => {
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
      .map((res) => {
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
      .map((res) => new Blob([res._body],{ type: 'application/vnd.ms-excel' }));
  }
}
