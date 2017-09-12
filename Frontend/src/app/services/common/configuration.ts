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
        
        this.UrlCRM = "http://azsof01wd:8098/api";

        this.GrantType = "password";
        this.ClientId = "b261e1b1-b207-4987-bdd8-1d65bc8e1286";
        this.Resource = "https://tebrasofre.onmicrosoft.com/02b049bf-c2db-404e-a1d7-22bf0ea5a332";
        this.TenantId = "0cd8cc48-a338-45eb-b01c-37c623d90a78";
    }

    setCurrLang(currLang: string){
        this.currLang = currLang;
        this.tr.use(this.currLang);
    }

    getHeaders(){
        let headers = new Headers();
        let token = Cookie.get('access_token');

        headers.append('Content-Type', 'application/json');
        headers.append('Access-Control-Allow-Origin', 'http://azsof01wd');
        headers.append('Authorization', token);
        
        return headers;
    }

    getLoginHeaders(){
        let headers = new Headers();
        headers.append('Content-Type', 'application/x-www-form-urlencoded');
        return headers;
    }
}