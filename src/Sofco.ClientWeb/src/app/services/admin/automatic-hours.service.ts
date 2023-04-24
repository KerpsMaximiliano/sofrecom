import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Service } from '../common/service';

@Injectable({
    providedIn: 'root'
})
export class AutomaticHoursService {

  private baseUrl: string;

  constructor(
    private http: HttpClient,
    private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getTasks() {
    return this.http.get<any>(`${this.baseUrl}/PreloadAutomaticWorkTimes/GetTasks`);
  }

  runProcess() {
    return this.http.get<any>(`${this.baseUrl}/PreloadAutomaticWorkTimes/RunProcess`);
  }
  
}
