
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "../common/service";
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

  getByTitle(title) {
    return this.http.get<any>(`${this.baseUrl}/analytics/title/${title}`);
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

  getOptionsActives(){
    return this.http.get<any>(`${this.baseUrl}/analytics/options/active`);
  }

  getClientId(clientId){
    return this.http.get<any>(`${this.baseUrl}/analytics/clients/${clientId}`);
  }

  getActivesByClientId(clientId){
    return this.http.get<any>(`${this.baseUrl}/analytics/clients/${clientId}/actives`);
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

  updateDaf(model) {
    return this.http.put<any>(`${this.baseUrl}/analytics/daf`, model);
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

  getByManager() {
    return this.http.get<any>(`${this.baseUrl}/analytics/options/currentManager`);
  }

  get(query) {
    return this.http.post<any>(`${this.baseUrl}/analytics/search`, query);
  }

  getOpportunities(id) {
    return this.http.get<any>(`${this.baseUrl}/analytics/${id}/opportunities`);
  }

  createReport(ids){
    return this.http.post(`${this.baseUrl}/analytics/report`, ids, {
      responseType: 'arraybuffer',
      observe: 'response'
    }).pipe(map((res: any) => {
      return new Blob([res.body], { type: 'application/octet-stream' });
    }));
  }
  reopen(id) {
    return this.http.put<any>(`${this.baseUrl}/analytics/${id}/reopen`, {});
  }
}
