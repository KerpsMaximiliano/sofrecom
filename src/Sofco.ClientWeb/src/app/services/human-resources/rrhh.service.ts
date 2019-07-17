import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Service } from "app/services/common/service";
import {map} from 'rxjs/operators';

@Injectable()
export class RrhhService {
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getTigerTxt() {
    return this.http.get(`${this.baseUrl}/rrhh/tiger/txt/`, {
        responseType: 'arraybuffer',
        observe: 'response'
    }).pipe(map((res: any) => {
        return new Blob([res.body], { type: 'application/octet-stream' });
    }));
  }

  syncSocialCharges(year, month) {
    return this.http.put<any>(`${this.baseUrl}/rrhh/${year}/${month}/socialCharges`, {});
  }
}