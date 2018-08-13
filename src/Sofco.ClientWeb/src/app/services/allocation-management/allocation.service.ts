import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';

@Injectable()
export class AllocationService {

  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll(employeeId, startDate, endDate) {
    return this.http.get<any>(`${this.baseUrl}/allocations/${employeeId}/${startDate}/${endDate}`);
  }

  getAllPercentages() {
    return this.http.get<any>(`${this.baseUrl}/allocations/percentages`);
  }

  getAllocations(employeeId, startDate, endDate) {
    return this.http.get<any>(`${this.baseUrl}/allocations/analytics/${employeeId}/${startDate}/${endDate}`);
  }

  add(model) {
    return this.http.post<any>(`${this.baseUrl}/allocations`, model);
  }

  addMassive(model){
    return this.http.post(`${this.baseUrl}/allocations/massive`, model, {
      responseType: 'arraybuffer',
      observe: 'response'
    }).pipe(map((res: any) => {
      return new Blob([res.body], { type: 'application/octet-stream' });
    }));
  }

  getAllocationsByService(serviceId) {
    return this.http.get<any>(`${this.baseUrl}/allocations/service/${serviceId}`);
  }

  getAllocationsByAnalytic(analyticId) {
    return this.http.get<any>(`${this.baseUrl}/allocations/analytic/${analyticId}`);
  }

  createReport(parameters) {
    return this.http.post<any>(`${this.baseUrl}/allocations/report`, parameters);
  }
}
