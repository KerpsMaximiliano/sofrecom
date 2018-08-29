import { Injectable } from '@angular/core';
import { Configuration } from "./configuration";

@Injectable()
export class  Service {

    UrlApi: string;

    constructor(private config: Configuration){
        this.UrlApi = config.UrlApi;
    }

    getHeaders() {
        return this.config.getHeaders();
    }

    getLoginHeaders() {
        return this.config.getLoginHeaders();
    }
}