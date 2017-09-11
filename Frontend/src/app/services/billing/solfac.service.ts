import { Injectable } from '@angular/core';
import { Http, Response, Headers } from '@angular/http';
import { Service } from "app/services/common/service";

@Injectable()
export class SolfacService {
  private baseUrl: string;
  private headers: Headers;

  constructor(private http: Http, private service: Service) {
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

  add(model){
     return this.http.post(`${this.baseUrl}/solfac`, model, { headers: this.headers}).map((res:Response) => res.json());
  }
}
