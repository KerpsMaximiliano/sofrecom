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

  get(date) {
    return this.http.get<any>(`${this.apiUrl}/${date}`);
  }

  post(model) {
    return this.http.post<any>(`${this.apiUrl}`, model);
  }
}