import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';
import { MenuService } from '../admin/menu.service';
import {map} from 'rxjs/operators';

@Injectable()
export class WorktimeService {
  private apiUrl: string;

  constructor(private http: HttpClient, private service: Service, private menuService: MenuService) {
    this.apiUrl = this.service.UrlApi + '/worktimes';
  }

  getWorkTimeApproved(model) {
    return this.http.post<any>(`${this.apiUrl}/hoursApproved`, model);
  }

  getWorkTimePending(model) {
    return this.http.post<any>(`${this.apiUrl}/hoursPending`, model);
  }

  get(date) {
    return this.http.get<any>(`${this.apiUrl}/${date}`);
  }

  getStatus() {
    return this.http.get<any>(`${this.apiUrl}/status`);
  }

  post(model) {
    return this.http.post<any>(`${this.apiUrl}`, model);
  }

  createReport(model) {
    return this.http.post<any>(`${this.apiUrl}/report`, model);
  }

  search(model) {
    return this.http.post<any>(`${this.apiUrl}/search`, model);
  }

  approve(id) {
    return this.http.put<any>(`${this.apiUrl}/${id}/approve`, {});
  }

  delete(id) {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }

  approveAll(hourIds){
    return this.http.put<any>(`${this.apiUrl}/approve`, hourIds);
  }

  rejectAll(model){
    return this.http.put<any>(`${this.apiUrl}/reject`, model);
  }

  reject(id, comments) {
    return this.http.put<any>(`${this.apiUrl}/${id}/reject`, { comments: comments });
  }

  getAnalytics(){
    return this.http.get<any>(`${this.apiUrl}/analytics`);
  }

  sendHours(){
    return this.http.put<any>(`${this.apiUrl}/send`, {});
  }

  getUrlForImportFile(analyticId){
    return `${this.apiUrl}/import/${analyticId}`;
  }

  exportTemplate(analyticId, closeMonthId){
    return this.http.get(`${this.apiUrl}/export/template/${analyticId}/${closeMonthId}`, {
      responseType: 'arraybuffer',
      observe: 'response'
    }).pipe(map((res: any) => {
      return new Blob([res.body], { type: 'application/octet-stream' });
    }));
  }

  adminUpdate(id, json) {
    return this.http.put<any>(`${this.apiUrl}/${id}/admin`, json);
  }
}