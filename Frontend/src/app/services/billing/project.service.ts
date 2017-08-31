import { Injectable } from '@angular/core';
import { Role } from "models/role";
import { Http, Response, Headers} from '@angular/http';
import { Service } from "app/services/service";

@Injectable()
export class ProjectService {

  private baseUrl: string;
  private headers: Headers;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlApi;
    this.headers = this.service.getHeaders();
  }

  getAll(serviceId: number) {
      return this.http.get(`${this.baseUrl}/project/${serviceId}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getHitos(projectId){
      return this.http.get(`${this.baseUrl}/project/${projectId}/hitos`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }

  getSolfacs(projectId){
      return this.http.get(`${this.baseUrl}/solfac/project/${projectId}`, { headers: this.service.getHeaders()}).map((res:Response) => res.json());
  }
}
