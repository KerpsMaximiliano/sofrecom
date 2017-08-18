import { MenuService } from 'app/services/menu.service';
import { Service } from 'app/services/service';
import { Injectable } from '@angular/core';
import { Http, Headers, Response } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';

@Injectable()
export class AuthenticationService {
    private baseUrl: string;
    private headers: Headers;

    constructor(private http: Http, 
                private service: Service,
                private menuService: MenuService) {
        this.baseUrl = this.service.UrlApi;
        this.headers = this.service.getHeaders();
    }

    login(username: string, password: string) {
        var json = { mail: username, password: password };

        //temporal hasta que esté el servicio
        return this.http.post(`${this.baseUrl}/login/`, json, { headers: this.headers}).map((res: Response) => res.json());

        /*
        return this.http.get(`${this.baseUrl}/user/1/detail`, { headers: this.headers}).map((res:Response) => {
                // login successful if there's a jwt token in the response
                let user = response.json();
                if (user && user.token) {
                    // store user details and jwt token in local storage to keep user logged in between page refreshes
                    localStorage.setItem('currentUser', JSON.stringify(user));
                }

                return user;
        });*/

        /*return this.http.post('/api/authenticate', JSON.stringify({ username: username, password: password }))
            .map((response: Response) => {
                // login successful if there's a jwt token in the response
                let user = response.json();
                if (user && user.token) {
                    // store user details and jwt token in local storage to keep user logged in between page refreshes
                    localStorage.setItem('currentUser', JSON.stringify(user));
                }

                return user;
            });*/
    }

    logout() {
        // remove user from local storage to log user out
        localStorage.removeItem('currentUser');
        localStorage.removeItem('menu');
    }
}