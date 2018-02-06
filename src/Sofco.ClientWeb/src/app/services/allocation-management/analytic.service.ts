import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class AnalyticService {

  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get(`${this.baseUrl}/analytics`).map((res:Response) => res.json());
  }

  getById(id) {
    return this.http.get(`${this.baseUrl}/analytics/${id}`).map((res:Response) => res.json());
  }

  getResources(id) {
    return this.http.get(`${this.baseUrl}/analytics/${id}/resources`).map((res:Response) => res.json());
  }

  getOptions(){
    return this.http.get(`${this.baseUrl}/analytics/options`).map((res:Response) => res.json());
  }

  getFormOptions(){
    return this.http.get(`${this.baseUrl}/analytics/formOptions`).map((res:Response) => res.json());
  }

  getNewTitle(costCenterId) {
    return this.http.get(`${this.baseUrl}/analytics/title/costcenter/${costCenterId}`).map((res:Response) => res.json());
  }

  add(model){
    return this.http.post(`${this.baseUrl}/analytics`, model).map((res:Response) => res.json());
  }

  update(model){
    return this.http.put(`${this.baseUrl}/analytics`, model).map((res:Response) => res.json());
  }
}
