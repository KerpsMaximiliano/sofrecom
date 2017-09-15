import { TranslateService } from '@ngx-translate/core';
import { Injectable } from '@angular/core';
import { Headers } from '@angular/http';
import { Cookie } from 'ng2-cookies/ng2-cookies';

@Injectable()
export class  Configuration {

    UrlApi : string;
    UrlApiNode: string;
    UrlCRM: string;
    public currLang: string = 'es';
    GrantType: string;
    ClientId: string;
    Resource: string;
    TenantId: string;

    constructor(public tr: TranslateService){
        tr.addLangs(["en", "es"]);
        let browserLang = tr.getBrowserLang();
        tr.setDefaultLang(browserLang);
        tr.use(this.currLang);

        //this.UrlApi = "http://azsof01wd:8081/api"  
        this.UrlApi = "http://localhost:9696/api" 
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
        headers.append('Authorization', token);
        
        return headers;
    }

    getLoginHeaders(){
        let headers = new Headers();
        headers.append('Access-Control-Allow-Origin', '*');
        return headers;
    }
}