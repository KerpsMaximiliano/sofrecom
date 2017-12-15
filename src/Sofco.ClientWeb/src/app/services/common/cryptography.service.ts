import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';

@Injectable()
export class CryptographyService {
    
    private key = CryptoJS.enc.Utf8.parse("zoMgRaMpAamuDezt");
    private iv = CryptoJS.enc.Utf8.parse("A0f08A80E#A08s8f");

    encrypt(text: string) {

        let params = { 
            iv: this.iv, 
            mode: CryptoJS.mode.CBC,
            padding: CryptoJS.pad.Pkcs7
        };

        var encrypted = CryptoJS.AES.encrypt(text, this.key, params);

        return encrypted.ciphertext.toString(CryptoJS.enc.Base64);
    }
}