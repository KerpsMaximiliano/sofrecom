import { Injectable } from '@angular/core';
import { Response, ResponseContentType } from '@angular/http';
import { Service } from "app/services/common/service";
import { HttpAuth } from "app/services/common/http-auth";

@Injectable()
export class CertificatesService {

  private baseUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get(`${this.baseUrl}/certificates`).map((res:Response) => res.json());
  }

  getById(id) {
    return this.http.get(`${this.baseUrl}/certificates/${id}`).map((res:Response) => res.json()); 
  }
 
  add(model){
    return this.http.post(`${this.baseUrl}/certificates`, model).map((res:Response) => res.json());
  }

  search(params){
    return this.http.post(`${this.baseUrl}/certificates/search`, params).map((res:Response) => res.json());
  }

  update(model){
    return this.http.put(`${this.baseUrl}/certificates`, model).map((res:Response) => res.json());
  }

  getUrlForImportFile(id){
    return `${this.baseUrl}/certificates/${id}/file`;
  }

  deleteFile(id) {
    return this.http.delete(`${this.baseUrl}/certificates/${id}/file`).map((res:Response) => res.json()); 
  }

  exportFile(id){
    return this.http.get(`${this.baseUrl}/certificates/export/${id}`,
     {
       responseType: ResponseContentType.Blob
     })
     .map((res:any) => {
       if(res.status >= 300 && res.status <= 500){
         return res.json();
       }
       else{
         return new Blob([res._body],{ type: 'application/octet-stream' })
       }
     });
 }
}

