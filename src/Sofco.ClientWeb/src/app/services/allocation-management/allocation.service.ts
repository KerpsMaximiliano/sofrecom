import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class AllocationService {

  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll(employeeId, startDate, endDate) {
    return this.http.get(`${this.baseUrl}/allocations/${employeeId}/${startDate}/${endDate}`).map((res:Response) => res.json());
  }

  getAllocations(employeeId, startDate, endDate) {
    return this.http.get(`${this.baseUrl}/allocations/analytics/${employeeId}/${startDate}/${endDate}`).map((res:Response) => res.json());
  }

  add(model){
    return this.http.post(`${this.baseUrl}/allocations`, model).map((res:Response) => res.json());
  }

  getAllocationsByService(serviceId) {
    return this.http.get(`${this.baseUrl}/allocations/service/${serviceId}`).map((res:Response) => res.json());
  }
}
