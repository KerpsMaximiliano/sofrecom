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

  get(year) {
    return this.http.get<any>(`${this.apiUrl}/${year}`);
  }

  post(model) {
    return this.http.post<any>(`${this.apiUrl}`, model);
  }

  importExternalData(year) {
    return this.http.post<any>(`${this.apiUrl}/importExternalData/${year}`, null);
  }

  delete(id: number) {
    return this.http.delete<any>(`${this.apiUrl}/${id}`);
  }
}
