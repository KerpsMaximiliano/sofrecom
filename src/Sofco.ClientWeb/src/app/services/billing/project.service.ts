import { Injectable } from '@angular/core';
import { Http, Response} from '@angular/http';
import { Service } from "app/services/common/service";

@Injectable()
export class ProjectService {

  private baseUrl: string;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll(serviceId) {
      return this.http.get(`${this.baseUrl}/project/service/${serviceId}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getById(projectId) {
      return this.http.get(`${this.baseUrl}/project/${projectId}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getOptions(serviceId) {  
    return this.http.get(`${this.baseUrl}/project/${serviceId}/options`, { headers: this.service.getHeaders() }).map((res:Response) => res.json());
  }

  getHitos(projectId){
      return this.http.get(`${this.baseUrl}/project/${projectId}/hitos`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getSolfacs(projectId){
      return this.http.get(`${this.baseUrl}/solfac/project/${projectId}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getInvoices(projectId){
      return this.http.get(`${this.baseUrl}/invoice/project/${projectId}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }
}
