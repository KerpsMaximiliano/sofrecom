import { Injectable } from '@angular/core';
import { Http, Response, Headers } from '@angular/http';
import { Service } from "app/services/common/service";

@Injectable()
export class InvoiceService {
  private baseUrl: string;
  private headers: Headers;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlApi;
    this.headers = this.service.getHeaders();
  }

  add(model){
     return this.http.post(`${this.baseUrl}/invoice`, model, { headers: this.headers}).map((res:Response) => res.json());
  }
}
