import { Injectable } from '@angular/core';
import { Service } from "app/services/common/service";
import { HttpClient } from '@angular/common/http';

@Injectable()
export class LicenseViewDelegateService {
    private apiUrl: string;

    constructor(private http: HttpClient, private service: Service) {
      this.apiUrl = `${this.service.UrlApi}/licenses/views/delegates`;
    }

    save(model) {
      return this.http.post<any>(this.apiUrl, model);
    }

    getAll() {
      return this.http.get<any>(this.apiUrl);
    }

    delete(id: number) {
      return this.http.delete<any>(this.apiUrl + '/' + id);
    }
}
