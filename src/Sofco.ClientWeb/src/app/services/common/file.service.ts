import { Injectable } from '@angular/core';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class FileService {
  private apiUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.apiUrl = this.service.UrlApi + '/file';
  }

  getFile(id, type) {
    return this.http.get<any>(`${this.apiUrl}/${id}/${type}`);
  }
}