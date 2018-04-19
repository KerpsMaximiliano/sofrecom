import { Injectable } from '@angular/core';
import { Response, Headers, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';
import { MenuService } from '../admin/menu.service';

@Injectable()
export class WorktimeService {
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service, private menuService: MenuService) {
    this.baseUrl = this.service.UrlApi;
  }

  getWorkTimeApproved(model) {
    return this.http.post<any>(`${this.baseUrl}/worktime/hoursApproved`, model);
  }
}