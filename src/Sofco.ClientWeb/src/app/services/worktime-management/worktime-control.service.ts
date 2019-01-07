import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class WorktimeControlService {
  private apiUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.apiUrl = this.service.UrlApi + '/worktimes/worktimeControls';
  }

  getWorkTimeApproved(model) {
    return this.http.post<any>(`${this.apiUrl}`, model);
  }

  GetAnalyticOptionsByCurrentManager() {
    return this.http.get<any>(`${this.apiUrl}/analytics/options/currentManager`);
  }
}
