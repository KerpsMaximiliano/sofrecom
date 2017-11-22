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

  add(model){
    return this.http.post(`${this.baseUrl}/costcenter`, model).map((res:Response) => res.json());
  }
}
