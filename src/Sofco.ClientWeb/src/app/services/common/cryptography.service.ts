import { Injectable } from '@angular/core';
import * as CryptoJS from 'crypto-js';

@Injectable()
export class CryptographyService {
    
    constructor() { }

    private readonly key: string = "";
    private readonly iv: number[] = [1, 2, 3, 4, 5];

    encrypt(data: string){
        var key = CryptoJS.enc.Utf8.parse(this.key);
        var iv = CryptoJS.enc.Utf8.parse(this.iv);

        var cipherData = CryptoJS.AES.encrypt(data, key, { iv: iv });

        return cipherData;
    }
}