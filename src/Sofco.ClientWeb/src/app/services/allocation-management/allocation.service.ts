import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

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

  getAllocationsByService(serviceId) {
    return this.http.get<any>(`${this.baseUrl}/allocations/service/${serviceId}`);
  }

  createReport(parameters) {
    return this.http.post<any>(`${this.baseUrl}/allocations/report`, parameters);
  }
}
