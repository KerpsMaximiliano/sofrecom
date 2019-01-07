import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Service } from "app/services/common/service";

@Injectable()
export class WorkflowService {
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getTransitions(model) {
    return this.http.post<any>(`${this.baseUrl}/${model.entityController}/possibleTransitions`, model);
  }

  post(model){
    return this.http.post<any>(`${this.baseUrl}/${model.entityController}/transition`, model);
  }
}