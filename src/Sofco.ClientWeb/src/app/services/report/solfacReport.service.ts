import { Injectable } from '@angular/core';
import { Response, Headers, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class SolfacReportService {
  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getData(parameters) {
    return this.http.post(`${this.baseUrl}/reports/solfacs`, parameters).map((res:Response) => res.json());
  }
}
