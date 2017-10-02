import { Response, RequestOptions, ConnectionBackend } from '@angular/http';
import { Observable } from 'rxjs/Observable';

import { HttpAuthInterceptor, InterceptorConfig } from './http-auth-interceptor';
import { Cookie } from 'ng2-cookies/ng2-cookies';

import { Service } from "./service";

export class HttpAuth extends HttpAuthInterceptor {
  private service: Service;

  constructor(
    backend: ConnectionBackend, 
    defaultOptions: RequestOptions,
    service: Service
  ) {
    super(backend, defaultOptions, new InterceptorConfig({ noTokenError: true }));
    this.service = service;
  }

  protected getToken(): string {
    return Cookie.get('access_token');
  }

  protected saveToken(data: any): string {
    Cookie.set('access_token', data.access_token);
    Cookie.set('refresh_token', data.refresh_token);
    return data.access_token;
  }

  protected refreshToken(): Observable<Response> {
    var refreshToken = Cookie.get('refresh_token');

    return this.post(`${this.service.UrlApi}/login/refresh`, { 
      refreshToken: refreshToken
    }, null, true);
  }
}