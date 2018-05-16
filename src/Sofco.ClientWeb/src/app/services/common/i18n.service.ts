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

    translate(folder, code) {
        const lang = this.config.currLang;

        if (!folder || folder == null || folder == '') return;

        const translateFile = require(`../../../assets/i18n/${lang}/${folder}.json`);

        if (!translateFile || !code) return "";

        let value = "";

        const keySplitted = code.split(".");

        if (keySplitted.length > 1) {

            const arraySliced = keySplitted.slice(1, keySplitted.length);

            let obj = translateFile[keySplitted[0]];

            arraySliced.forEach(element => {
                obj = obj[element];
            });

            if (obj) return obj;
        } else {
            const rawCode = this.trimCode(code);

            value = this.processWithParameters(code, translateFile[rawCode]);

            if (value) return value;
        }

        return "";
    }

    trimCode(code: string): string {
        if (code.indexOf('?') > -1) {
            code = code.split('?')[0];
        }

        return code;
    }

    processWithParameters(code: string, msg: any): string {
        let parms = null;

        if (code.indexOf('?') > -1) {
            parms = code.split('?')[1].split(',');
        }

        if (parms != null) {
            msg = msg.format(...parms);
        }

        return msg;
    }
}
