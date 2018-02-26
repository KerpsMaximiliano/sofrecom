import { Injectable } from '@angular/core';
import { Response, Headers, ResponseContentType } from '@angular/http';
import { Service } from 'app/services/common/service';
import { HttpAuth } from 'app/services/common/http-auth';

@Injectable()
export class SolfacDelegateService {
  private apiUrl: string;

  constructor(private http: HttpAuth, private service: Service) {
    this.apiUrl = `${this.service.UrlApi}/solfacs/delegates`;
  }

  save(model) {
    return this.http.post(this.apiUrl, model).map((res: Response) => res.json());
  }
}
