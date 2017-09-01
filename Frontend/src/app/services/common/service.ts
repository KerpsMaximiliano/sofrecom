import { Injectable } from '@angular/core';
import { Headers } from '@angular/http';
import { Configuration } from "app/services/common/configuration";

@Injectable()
export class  Service {

    UrlApi : string;
    UrlApiNode: string;
    UrlCRM: string;
    UrlAzure: string;
    GrantType: string;
    ClientId: string;
    Resource: string;

    constructor(private config: Configuration){

        this.UrlApi = config.UrlApi;
        this.UrlApiNode = config.UrlApiNode;
        this.UrlCRM = config.UrlCRM;
        this.GrantType = config.GrantType;
        this.ClientId = config.ClientId;
        this.Resource = config.Resource;
        this.UrlAzure = `https://login.windows.net/${config.TenantId}/oauth2/token?api-version=1.1`;
    }

    getHeaders(){
        return this.config.getHeaders();
    }

    getLoginHeaders(){
        return this.config.getLoginHeaders();
    }
}