import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ContractService {

  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAccountInfo(year, month) {
    return this.http.get<any>(`${this.baseUrl}/contract/accounts/${year}/${month}`);
  }
}
