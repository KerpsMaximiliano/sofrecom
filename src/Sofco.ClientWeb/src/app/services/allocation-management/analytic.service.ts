import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class AnalyticService {

  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get<any>(`${this.baseUrl}/analytics`);
  }

  getById(id) {
    return this.http.get<any>(`${this.baseUrl}/analytics/${id}`);
  }

  getTimelineResources(id, dateSince, months) {
    return this.http.get<any>(`${this.baseUrl}/analytics/${id}/resources/timeline/${dateSince}/${months}`);
  }

  getResources(id) {
    return this.http.get<any>(`${this.baseUrl}/analytics/${id}/resources`);
  }

  getOptions(){
    return this.http.get<any>(`${this.baseUrl}/analytics/options`);
  }

  getClientId(clientId){
    return this.http.get<any>(`${this.baseUrl}/analytics/clients/${clientId}`);
  }

  getFormOptions(){
    return this.http.get<any>(`${this.baseUrl}/analytics/formOptions`);
  }

  getNewTitle(costCenterId) {
    return this.http.get<any>(`${this.baseUrl}/analytics/title/costcenter/${costCenterId}`);
  }

  add(model) {
    return this.http.post<any>(`${this.baseUrl}/analytics`, model);
  }

  update(model) {
    return this.http.put<any>(`${this.baseUrl}/analytics`, model);
  }

  close(id) {
    return this.http.put<any>(`${this.baseUrl}/analytics/${id}/close`, {});
  }

  closeForExpenses(id) {
    return this.http.put<any>(`${this.baseUrl}/analytics/${id}/closeForExpenses`, {});
  }

  getByCurrentUser() {
    return this.http.get<any>(`${this.baseUrl}/analytics/options/currentUser`);
  }

  get(query) {
    return this.http.post<any>(`${this.baseUrl}/analytics/search`, query);
  }

  createReport(ids){
    return this.http.post(`${this.baseUrl}/analytics/report`, ids, {
      responseType: 'arraybuffer',
      observe: 'response'
    }).map((res: any) => {
      return new Blob([res.body], { type: 'application/octet-stream' });
    });
  }
}
