import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from 'app/services/common/service';
import { HttpAuth } from 'app/services/common/http-auth';

@Injectable()
export class CustomerService {

  private apiUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.apiUrl = this.service.UrlApi + '/customers';
  }

  getAll() {
    return this.http.get(this.apiUrl).map((res: Response) => res.json());
  }

  getOptions() {
    return this.http.get(`${this.apiUrl}/options`).map((res: Response) => res.json().data);
  }

  getById(customerId) {
    return this.http.get(`${this.apiUrl}/${customerId}`).map((res: Response) => res.json().data);
  }
}
