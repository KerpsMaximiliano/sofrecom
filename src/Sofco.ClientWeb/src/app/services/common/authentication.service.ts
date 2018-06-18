import { Injectable } from '@angular/core';
import { Http, Response } from '@angular/http';
import 'rxjs/add/operator/map';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { Service } from "app/services/common/service";
import { CryptographyService } from "app/services/common/cryptography.service";

@Injectable()
export class AuthenticationService {
    constructor(private http: Http,
                private service: Service,
                private cryptoService: CryptographyService) {
    }

    login(username: string, password: string) {
        const json = {
            userName: username,
            password: this.cryptoService.encrypt(password)
        };

        return this.http.post(`${this.service.UrlApi}/login`, json, { headers: this.service.getLoginHeaders()}).map(this.loginResponseHanlder);
    }

    loginResponseHanlder(response: Response)
    {
        return response.json().data;
    }

    logout() {
        Cookie.deleteAll();
        sessionStorage.clear();
        localStorage.removeItem('menu');
        localStorage.removeItem('mustLogout');
    }
}