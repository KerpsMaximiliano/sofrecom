import { Component, Injectable } from '@angular/core';
import { Http, Response, Headers, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs';
import { Service } from './service';
import 'rxjs/add/operator/map';

import { Customer } from 'models/customer';

@Injectable()
export class CustomersService {

  private baseUrl: string;
  private headers: Headers;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlApi;
    this.headers = this.service.getHeaders();
  }

  getAll() {
    return this.http.get(`${this.baseUrl}/Customer`, { headers: this.headers}).map((res:Response) => res.json());
  }

  get(id: string) {
    return this.http.get(`${this.baseUrl}/customer/${id}`, { headers: this.headers}).map((res:Response) => res.json());
  }

  save(model : Customer) {
    return this.http.post(`${this.baseUrl}/Customer`, model, { headers: this.headers}).map((res:Response) => res );
  }

}
