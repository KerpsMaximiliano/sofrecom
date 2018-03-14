import { Observable } from 'rxjs/Observable';
import { Cookie } from 'ng2-cookies/src/services';
import 'rxjs/add/observable/of';
import 'rxjs/add/operator/delay';
import { Http } from '@angular/http';
import { Service } from './service';
import { Injectable } from '@angular/core';

@Injectable()
export class AuthService {
    constructor(private httpClient: Http,
        private service: Service) {
    }

    getAuthToken() {
        return Cookie.get('access_token');
    }

    refreshToken(): Observable<string> {
       console.log('Token expired, resfreh token called.');

       const refreshToken = Cookie.get('refresh_token');

       const response = this.httpClient.post(`${this.service.UrlApi}/login/refresh`, {
         refreshToken: refreshToken
       });

        return response.map(res => {
            const dataJson = JSON.parse(res['_body']).data;

            const data = JSON.parse(dataJson);

            Cookie.set('access_token', data.access_token);
            Cookie.set('refresh_token', data.refresh_token);

            return data.access_token;
        });
    }
}
