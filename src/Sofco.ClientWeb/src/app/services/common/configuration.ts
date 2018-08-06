import { TranslateService } from '@ngx-translate/core';
import { Injectable } from '@angular/core';
import { Headers } from '@angular/http';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { environment } from '../../../environments/environment'

@Injectable()
export class Configuration {

    UrlApi: string;
    crmCloseStatusCode: string;

    public currLang = 'es';

    constructor(public tr: TranslateService){
        this.UrlApi = environment.urlApi;
        this.crmCloseStatusCode = environment.crmCloseStatusCode;
    }

    setCurrLang(currLang: string){
        this.currLang = currLang;
        this.tr.use(this.currLang);
        localStorage.setItem('lang', this.currLang);
    }

    getHeaders(){
        const headers = new Headers();
        const token = Cookie.get('access_token');

        headers.append('Content-Type', 'application/json');
        headers.append('Access-Control-Allow-Origin', '*');
        headers.append('Authorization', 'Bearer '+ token);

        return headers;
    }

    getLoginHeaders(){
        const headers = new Headers();
        headers.append('Access-Control-Allow-Origin', '*');
        return headers;
    }
}