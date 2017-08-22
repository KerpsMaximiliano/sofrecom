import { Injectable } from '@angular/core';
import { Role } from "models/role";
import { Http, Response} from '@angular/http';
import { Service } from "app/services/service";

@Injectable()
export class ProjectService {

  private baseUrl: string;

  constructor(private http: Http, private service: Service) {
    this.baseUrl = this.service.UrlCRM;
  }

  getAll(serviceId: number) {
       return this.http.get(`${this.baseUrl}/project?idService=${serviceId}`).map((res:Response) => res.json());
  }
}
