import { Injectable } from '@angular/core';
import { Response, Headers, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class SolfacReportService {
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getData(parameters) {
    return this.http.post<any>(`${this.baseUrl}/reports/solfacs`, parameters);
  }
}
