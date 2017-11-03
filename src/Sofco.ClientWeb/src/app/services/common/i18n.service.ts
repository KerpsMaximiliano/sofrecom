import { Injectable } from '@angular/core';
import { Configuration } from 'app/services/common/configuration';
declare function require(name: string);

@Injectable()
export class I18nService {

    constructor(private config: Configuration) { }
 
    translateByKey(key){
        var lang = this.config.currLang;

        var translateFile = require(`../../../assets/i18n/${lang}.json`);

        var value = "";

        if(!translateFile) return value;

        if(key){
            var keySplitted = key.split(".");

            if(keySplitted.length > 1){

                var arraySliced = keySplitted.slice(1, keySplitted.length);
                
                var obj = translateFile[keySplitted[0]];

                arraySliced.forEach(element => {
                    obj = obj[element];
                });

                if(obj) return obj;
            }
            else{
                value = translateFile[key];
                if(value) return value;
            }
        }

        return "";

    }

    translate(folder, code){
        var lang = this.config.currLang;

        var translateFile = require(`../../../assets/i18n/${lang}/${folder}.json`);

        var value = "";

        if(!translateFile) return value;

        if(code){
            var keySplitted = code.split(".");

            if(keySplitted.length > 1){

                var arraySliced = keySplitted.slice(1, keySplitted.length);
                
                var obj = translateFile[keySplitted[0]];

                arraySliced.forEach(element => {
                    obj = obj[element];
                });

                if(obj) return obj;
            }
            else{
                value = translateFile[code];
                if(value) return value;
            }
        }

        return "";

    }
}