import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';

@Injectable()
export class WorktimeControlService {
  private apiUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.apiUrl = this.service.UrlApi + '/worktimes/worktimeControls';
  }

  getWorkTimeApproved(model) {
    return this.http.post<any>(`${this.apiUrl}`, model);
  }

  getAnalyticOptionsByCurrentManager() {
    return this.http.get<any>(`${this.apiUrl}/analytics/options/currentManager`);
  }

  createReport(){
    return this.http.get(`${this.apiUrl}/export`, {
      responseType: 'arraybuffer',
      observe: 'response'
    }).pipe(map((res: any) => {
      return new Blob([res.body], { type: 'application/octet-stream' });
    }));
  }
}
