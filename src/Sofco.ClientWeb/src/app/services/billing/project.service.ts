import { Injectable } from '@angular/core';
import { Http, Response} from '@angular/http';
import { Service } from 'app/services/common/service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class ProjectService {

  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll(serviceId) {
      return this.http.get<any>(`${this.baseUrl}/projects/service/${serviceId}`);
  }

  getById(projectId) {
      return this.http.get<any>(`${this.baseUrl}/projects/${projectId}`);
  }

  getOptions(serviceId) {
    return this.http.get<any>(`${this.baseUrl}/projects/${serviceId}/options`);
  }

  getHitos(projectId) {
      return this.http.get<any>(`${this.baseUrl}/projects/${projectId}/hitos`);
  }

  getSolfacs(projectId) {
      return this.http.get<any>(`${this.baseUrl}/solfacs/project/${projectId}`);
  }

  getInvoices(projectId) {
      return this.http.get<any>(`${this.baseUrl}/invoices/project/${projectId}`);
  }

  createNewHito(hito) {
    return this.http.post<any>(`${this.baseUrl}/hitos`, hito);
  }

  spltHito(hito) {
    return this.http.post<any>(`${this.baseUrl}/hitos/split`, hito);
  }

  closeHito(id) {
    return this.http.put<any>(`${this.baseUrl}/hitos/${id}/close`, {});
  }

  getIfIsRelated(serviceId) {
    return this.http.get<any>(`${this.baseUrl}/services/${serviceId}/analytic`);
  }
}
