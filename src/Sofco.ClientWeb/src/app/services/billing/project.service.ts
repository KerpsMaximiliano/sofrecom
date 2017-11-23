import { Injectable } from '@angular/core';
import { Http, Response} from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class ProjectService {

  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll(serviceId) {
      return this.http.get(`${this.baseUrl}/projects/service/${serviceId}`).map((res:Response) => res.json());
  }

  getById(projectId) {
      return this.http.get(`${this.baseUrl}/projects/${projectId}`).map((res:Response) => res.json());
  }

  getOptions(serviceId) {  
    return this.http.get(`${this.baseUrl}/projects/${serviceId}/options`).map((res:Response) => res.json());
  }

  getHitos(projectId){
      return this.http.get(`${this.baseUrl}/projects/${projectId}/hitos`).map((res:Response) => res.json());
  }

  getSolfacs(projectId){
      return this.http.get(`${this.baseUrl}/solfacs/project/${projectId}`).map((res:Response) => res.json());
  }

  getInvoices(projectId){
      return this.http.get(`${this.baseUrl}/invoices/project/${projectId}`).map((res:Response) => res.json());
  }

  createNewHito(hito){
    return this.http.post(`${this.baseUrl}/projects/hitos/new`, hito).map((res:Response) => res.json());
  }
}
