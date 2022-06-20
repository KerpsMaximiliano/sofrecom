import { Injectable } from "@angular/core";

@Injectable({
    providedIn: 'root'
})

export class ProvidersService {

    private mode: string;

    constructor() {}

    public getMode() {
        return this.mode;
    }

    public setMode(mode: string) {
        this.mode = mode;
    }
}