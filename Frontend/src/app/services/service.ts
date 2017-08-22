import { Configuration } from './configuration';
import { Injectable } from '@angular/core';
import { Headers } from '@angular/http';

@Injectable()
export class  Service {

    UrlApi : string;
    UrlApiNode: string;
    UrlCRM: string;
    Authorization: string;

    constructor(private config: Configuration){

        this.UrlApi = config.UrlApi;
        this.UrlApiNode = config.UrlApiNode;
        this.UrlCRM = config.UrlCRM;
        this.Authorization = config.Authorization;
    }

    getHeaders(){
        return this.config.getHeaders();
    }
}