import { TranslateService } from '@ngx-translate/core';
import { Injectable } from '@angular/core';
import { Headers } from '@angular/http';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { environment } from '../../../environments/environment'

@Injectable()
export class  Configuration {

    UrlApi : string;

    public currLang: string = 'es';

    constructor(public tr: TranslateService){
        tr.addLangs(["en", "es", "fr"]);
        let browserLang = tr.getBrowserLang();
        tr.setDefaultLang(browserLang);
        tr.use(this.currLang);

        this.UrlApi = environment.urlApi;
    }

    setCurrLang(currLang: string){
        this.currLang = currLang;
        this.tr.use(this.currLang);
    }

    getHeaders(){
        let headers = new Headers();
        let token = Cookie.get('access_token');

        headers.append('Content-Type', 'application/json');
        headers.append('Access-Control-Allow-Origin', '*');
        headers.append('Authorization', 'Bearer '+ token);
        
        return headers;
    }

    getLoginHeaders(){
        let headers = new Headers();
        headers.append('Access-Control-Allow-Origin', '*');
        return headers;
    }
}