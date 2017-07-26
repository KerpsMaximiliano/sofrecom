import { TranslateService } from '@ngx-translate/core';
import { Injectable } from '@angular/core';
import { Headers } from '@angular/http';

@Injectable()
export class  Configuration {

    UrlApi : string;
    Authorization: string;
    public currLang: string = 'en';


    constructor(public tr: TranslateService){
        tr.addLangs(["en", "es"]);
        tr.setDefaultLang(this.currLang);
        tr.use(this.currLang);

        this.UrlApi = "http://localhost:5000/api"; //dev
        //this.UrlApi = "http://sofrelab-iis1.cloudapp.net:9696/api"; // uat iis
        //this.UrlApi = "http://sofrelab-iis1.cloudapp.net:9000/api"; // uat

        this.Authorization="29c05029-ddf4-4e5e-8a91-62c5af6ae294";
    }

    setCurrLang(currLang: string){
        this.currLang = currLang;
        this.tr.use(this.currLang);
    }

    getHeaders(){
        let headers = new Headers();
        headers.append('Content-Type', 'application/json');
        headers.append('Authorization', '29c05029-ddf4-4e5e-8a91-62c5af6ae294');
        
        return headers;
    }
}