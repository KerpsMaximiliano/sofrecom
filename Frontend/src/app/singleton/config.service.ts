import { TranslateService, TranslatePipe } from '@ngx-translate/core';
import {ChangeDetectorRef} from '@angular/core';


import { Injectable } from "@angular/core";

@Injectable()
export class ConfigService{

    public currLang: string = 'en';

    private translatePipe: TranslatePipe;

    constructor(public tr: TranslateService){
        tr.addLangs(["en", "es"]);
        tr.setDefaultLang(this.currLang);
        tr.use(this.currLang);
    }

    setCurrLang(currLang: string){
        this.currLang = currLang;
        this.tr.use(this.currLang);
    }

    
}