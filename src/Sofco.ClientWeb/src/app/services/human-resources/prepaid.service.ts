import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Service } from "app/services/common/service";

@Injectable()
export class PrepaidService {
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getDashboard(year, month) {
    return this.http.get<any>(`${this.baseUrl}/prepaid/${year}/${month}/dashboard`);
  }

  get(year, month) {
    return this.http.get<any>(`${this.baseUrl}/prepaid/${year}/${month}`);
  }

  update(model) {
    return this.http.put<any>(`${this.baseUrl}/prepaid`, model);
  }
}