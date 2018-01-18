import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class NewsService {

  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get(`${this.baseUrl}/news`).map((res:Response) => res.json());
  }

  delete(id){
    return this.http.delete(`${this.baseUrl}/news/${id}`).map((res:Response) => res.json());
  }
}
