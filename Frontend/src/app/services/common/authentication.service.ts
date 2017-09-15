import { Injectable } from '@angular/core';
import { Http, Headers, Response, URLSearchParams } from '@angular/http';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/map';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { Service } from "app/services/common/service";
import { MenuService } from "app/services/admin/menu.service";

@Injectable()
export class AuthenticationService {
    private baseUrl: string;

    constructor(private http: Http, 
                private service: Service,
                private menuService: MenuService) {
        this.baseUrl = this.service.UrlApi;
    }

    login(username: string, password: string) {
        var json = {
            userName: username,
            password: password
        }

        //temporal hasta que esté el servicio
        return this.http.post(`${this.service.UrlApi}/login`, json, { headers: this.service.getLoginHeaders()}).map((res: Response) => res.json());
    }

    logout() {
        Cookie.deleteAll();
        sessionStorage.clear();
        localStorage.removeItem('menu');
    }
}