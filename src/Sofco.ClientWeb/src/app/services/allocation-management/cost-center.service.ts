import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class CostCenterService {

  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get(`${this.baseUrl}/costcenter`).map((res:Response) => res.json());
  }

  getById(id) {
    return this.http.get(`${this.baseUrl}/costcenter/${id}`).map((res:Response) => res.json());
  }

  getOptions() {
    return this.http.get(`${this.baseUrl}/costcenter/options`).map((res:Response) => res.json());
  }

  add(model){
    return this.http.post(`${this.baseUrl}/costcenter`, model).map((res:Response) => res.json());
  }

  edit(model){
    return this.http.put(`${this.baseUrl}/costcenter`, model).map((res:Response) => res.json());
  }

  changeStatus(id, active){
    return this.http.put(`${this.baseUrl}/costcenter/${id}/active/${active}`, {}).map((res:Response) => res.json());
  }
}
