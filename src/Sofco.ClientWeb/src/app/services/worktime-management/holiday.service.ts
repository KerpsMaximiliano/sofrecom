import { Injectable } from '@angular/core';
import { Response, Headers, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';
import { MenuService } from '../admin/menu.service';

@Injectable()
export class HolidayService {
  private apiUrl: string;

  constructor(private http: HttpClient, private service: Service, private menuService: MenuService) {
    this.apiUrl = this.service.UrlApi + '/holidays';
  }

  get() {
    return this.http.get<any>(`${this.apiUrl}/2018`);
  }

  post(model) {
    return this.http.post<any>(`${this.apiUrl}`, model);
  }
}
