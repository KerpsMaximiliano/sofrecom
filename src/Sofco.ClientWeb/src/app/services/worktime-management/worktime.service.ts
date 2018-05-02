import { Injectable } from '@angular/core';
import { Response, Headers, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';
import { MenuService } from '../admin/menu.service';

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

  post(model) {
    return this.http.post<any>(`${this.apiUrl}`, model);
  }

  approve(id) {
    return this.http.put<any>(`${this.apiUrl}/${id}/approve`, {});
  }

  approveAll(hourIds){
    return this.http.put<any>(`${this.apiUrl}/approve`, hourIds);
  }

  reject(id, comments) {
    return this.http.put<any>(`${this.apiUrl}/${id}/reject`, { comments: comments });
  }

  getAnalytics(){
    return this.http.get<any>(`${this.apiUrl}/analytics`);
  }
}