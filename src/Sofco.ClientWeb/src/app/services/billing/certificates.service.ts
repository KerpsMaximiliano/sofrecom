
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class CertificatesService {

  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get<any>(`${this.baseUrl}/certificates`);
  }

  getById(id) {
    return this.http.get<any>(`${this.baseUrl}/certificates/${id}`);
  }

  add(model) {
    return this.http.post<any>(`${this.baseUrl}/certificates`, model);
  }

  search(params) {
    return this.http.post<any>(`${this.baseUrl}/certificates/search`, params);
  }

  update(model) {
    return this.http.put<any>(`${this.baseUrl}/certificates`, model);
  }

  getUrlForImportFile(id) {
    return `${this.baseUrl}/certificates/${id}/file`;
  }

  deleteFile(id) {
    return this.http.delete<any>(`${this.baseUrl}/certificates/${id}/file`);
  }

  getByClient(client) {
    return this.http.get<any>(`${this.baseUrl}/certificates/client/${client}`);
  }

  getFile(id){
    return this.http.get<any>(`${this.baseUrl}/certificates/${id}/file`);
  }

  exportFile(id) {
    return this.http.get(`${this.baseUrl}/certificates/export/${id}`, {
      responseType: 'arraybuffer',
      observe: 'response'
   }).pipe(map((res: any) => {
     return new Blob([res.body], { type: 'application/octet-stream' });
   }));
 }
}

