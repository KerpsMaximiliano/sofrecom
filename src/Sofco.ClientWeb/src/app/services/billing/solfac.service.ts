import { Injectable } from '@angular/core';
import { Http, Response, Headers, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { MenuService } from 'app/services/admin/menu.service';

@Injectable()
export class SolfacService {
  private baseUrl: string;
  private headers: Headers;

  constructor(private http: Http, private service: Service, private menuService: MenuService) {
    this.baseUrl = this.service.UrlApi;
    this.headers = this.service.getHeaders();
  }

  getOptions() {
    return this.http.get(`${this.baseUrl}/solfacs/options`, { headers: this.headers}).map((res:Response) => res.json());
  }

  search(parameters) {
    return this.http.post(`${this.baseUrl}/solfacs/search`, parameters, { headers: this.headers}).map((res:Response) => res.json());
  }

  get(solfacId) {
    return this.http.get(`${this.baseUrl}/solfacs/${solfacId}`, { headers: this.headers}).map((res:Response) => res.json());
  }

  getStatus() {
    return this.http.get(`${this.baseUrl}/solfacs/status`, { headers: this.headers}).map((res:Response) => res.json());
  }

  getHistories(solfacId) {
    return this.http.get(`${this.baseUrl}/solfacs/${solfacId}/histories`, { headers: this.headers}).map((res:Response) => res.json());
  }

  getAttachments(solfacId) {
    return this.http.get(`${this.baseUrl}/solfacs/${solfacId}/files`, { headers: this.headers}).map((res:Response) => res.json());
  }

  add(model){
     return this.http.post(`${this.baseUrl}/solfacs`, model, { headers: this.headers}).map((res:Response) => res.json());
  }

  update(model){
    return this.http.put(`${this.baseUrl}/solfacs`, model, { headers: this.headers}).map((res:Response) => res.json());
  }

  send(model){
     return this.http.post(`${this.baseUrl}/solfacs/send`, model, { headers: this.headers}).map((res:Response) => res.json());
  }

  changeStatus(id, status, comment, invoiceCode){
     var body = {
       userId: this.menuService.user.id,
       comment: comment,
       status: status,
       invoiceCode: invoiceCode
     }

     return this.http.post(`${this.baseUrl}/solfacs/${id}/status`, body, { headers: this.headers}).map((res:Response) => res.json());
  }

  delete(id){
     return this.http.delete(`${this.baseUrl}/solfacs/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  deleteFile(id){
    return this.http.delete(`${this.baseUrl}/solfacs/file/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
 }

  getUrlForImportFile(id){
    return `${this.baseUrl}/solfacs/${id}/file`;
  }

  getFile(id){
    return this.http.get(`${this.baseUrl}/solfacs/file/${id}`,
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
}
