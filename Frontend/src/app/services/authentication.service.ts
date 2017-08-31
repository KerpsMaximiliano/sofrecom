import { MenuService } from 'app/services/menu.service';
import { Service } from 'app/services/service';
import { Injectable } from '@angular/core';
import { Http, Headers, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import { Cookie } from 'ng2-cookies/ng2-cookies';

@Injectable()
export class AuthenticationService {
    private baseUrl: string;
    private headers: Headers;

    constructor(private http: Http, 
                private service: Service,
                private menuService: MenuService) {
        this.baseUrl = this.service.UrlApi;
        this.headers = this.service.getLoginHeaders();
    }

    login(username: string, password: string) {
        let body = new URLSearchParams();
        body.set("username", username);
        body.set("password", password);
        body.set("grant_type", this.service.GrantType);
        body.set("client_id", this.service.ClientId);
        body.set("resource", this.service.Resource);

        //temporal hasta que esté el servicio
        return this.http.post(this.service.UrlAzure, body, { headers: this.headers}).map((res: Response) => res.json());
    }

    logout() {
        Cookie.delete('access_token');
        Cookie.delete('userInfo');
        Cookie.delete('currentUser');
        Cookie.delete('currentUserMail');
        localStorage.removeItem('menu');
    }
}