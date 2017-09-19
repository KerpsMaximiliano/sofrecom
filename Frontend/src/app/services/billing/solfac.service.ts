import { Injectable } from '@angular/core';
import { Http, Response, Headers } from '@angular/http';
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
    return this.http.get(`${this.baseUrl}/solfac/options`, { headers: this.headers}).map((res:Response) => res.json());
  }

  search(parameters) {
    return this.http.post(`${this.baseUrl}/solfac/search`, parameters, { headers: this.headers}).map((res:Response) => res.json());
  }

  get(solfacId) {
    return this.http.get(`${this.baseUrl}/solfac/${solfacId}`, { headers: this.headers}).map((res:Response) => res.json());
  }

  getHistories(solfacId) {
    return this.http.get(`${this.baseUrl}/solfac/${solfacId}/histories`, { headers: this.headers}).map((res:Response) => res.json());
  }

  add(model){
     return this.http.post(`${this.baseUrl}/solfac`, model, { headers: this.headers}).map((res:Response) => res.json());
  }

  send(model){
     return this.http.post(`${this.baseUrl}/solfac/send`, model, { headers: this.headers}).map((res:Response) => res.json());
  }

  changeStatus(id, status, comment){
     var body = {
       userId: this.menuService.user.id,
       comment: comment,
       status: status
     }

     return this.http.post(`${this.baseUrl}/solfac/${id}/status`, body, { headers: this.headers}).map((res:Response) => res.json());
  }

  delete(id){
     return this.http.delete(`${this.baseUrl}/solfac/${id}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }
}
