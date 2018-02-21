import { Injectable } from '@angular/core';
import { Response, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class PurchaseOrderService {

  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get(`${this.baseUrl}/purchaseOrders`).map((res:Response) => res.json());
  }

  getStatuses() {
    return this.http.get(`${this.baseUrl}/purchaseOrders/status`).map((res:Response) => res.json());
  }

  getFormOptions() {
    return this.http.get(`${this.baseUrl}/purchaseOrders/formOptions`).map((res:Response) => res.json());
  }

  getById(id) {
    return this.http.get(`${this.baseUrl}/purchaseOrders/${id}`).map((res:Response) => res.json()); 
  }

  add(model){
    return this.http.post(`${this.baseUrl}/purchaseOrders`, model).map((res:Response) => res.json());
  }

  search(params){
    return this.http.post(`${this.baseUrl}/purchaseOrders/search`, params).map((res:Response) => res.json());
  }

  update(model){
    return this.http.put(`${this.baseUrl}/purchaseOrders`, model).map((res:Response) => res.json());
  }

  getUrlForImportExcel(id){
    return `${this.baseUrl}/purchaseOrders/${id}/file`;
  }

  deleteFile(id) {
    return this.http.delete(`${this.baseUrl}/purchaseOrders/${id}/file`).map((res:Response) => res.json()); 
  }

  exportFile(id){
    return this.http.get(`${this.baseUrl}/purchaseOrders/export/${id}`,
     {
       responseType: ResponseContentType.Blob
     })
     .map((res:any) => {
       if(res.status >= 300 && res.status <= 500){
         return res.json();
       }
       else{
         return new Blob([res._body],{ type: 'application/octet-stream' })
       }
     });
 }
}

