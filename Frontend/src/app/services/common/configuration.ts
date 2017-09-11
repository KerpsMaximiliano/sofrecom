import { TranslateService } from '@ngx-translate/core';
import { Injectable } from '@angular/core';
import { Headers } from '@angular/http';
import { Cookie } from 'ng2-cookies/ng2-cookies';

declare function require(name:string);
var config = require('../../../assets/config/config.json');

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

        this.UrlApi = config.UrlApi;
        this.UrlCRM = config.UrlCRM;

        //"UrlApi": "http://azsof01wd:8081/api"  DEV
        //"UrlApi": "http://localhost:9696/api"  LOCALHOST

        this.GrantType = config.GrantType;
        this.ClientId = config.ClientId;
        this.Resource = config.Resource;
        this.TenantId = config.TenantId;
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