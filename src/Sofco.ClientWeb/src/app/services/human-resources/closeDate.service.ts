import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Service } from "app/services/common/service";

@Injectable()
export class CloseDateService {
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  get(startYear, startMonth, endYear, endMonth) {
    return this.http.get<any>(`${this.baseUrl}/closeDate/${startMonth}/${startYear}/${endMonth}/${endYear}`);
  }

  post(model) {
    return this.http.post<any>(`${this.baseUrl}/closeDate`, model);
  }
  getAllBeforeNextMonth(){
    return this.http.get<any>(`${this.baseUrl}/closeDate/GetFirstBeforeNextMonth`);
  }
}