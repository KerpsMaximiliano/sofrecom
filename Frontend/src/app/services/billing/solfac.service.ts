import { Injectable } from '@angular/core';
import { Http, Response, Headers } from '@angular/http';
import { Service } from "app/services/service";

@Injectable()
export class SolfacService {
  private baseUrl: string;
  private headers: Headers;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlApi;
    this.headers = this.service.getHeaders();
  }

  getOptions() {
    return this.http.get(`${this.baseUrl}/solfac/options`).map((res:Response) => res.json());
  }

  getAll() {
    return this.http.get(`${this.baseUrl}/solfac`).map((res:Response) => res.json());
  }

  add(model){
     return this.http.post(`${this.baseUrl}/solfac`, model, { headers: this.headers}).map((res:Response) => res.json());
  }
}
