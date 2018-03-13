import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class CostCenterService {

  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get<any>(`${this.baseUrl}/costcenter`);
  }

  getById(id) {
    return this.http.get<any>(`${this.baseUrl}/costcenter/${id}`);
  }

  getOptions() {
    return this.http.get<any>(`${this.baseUrl}/costcenter/options`);
  }

  add(model) {
    return this.http.post<any>(`${this.baseUrl}/costcenter`, model);
  }

  edit(model) {
    return this.http.put<any>(`${this.baseUrl}/costcenter`, model);
  }

  changeStatus(id, active) {
    return this.http.put<any>(`${this.baseUrl}/costcenter/${id}/active/${active}`, {});
  }
}
