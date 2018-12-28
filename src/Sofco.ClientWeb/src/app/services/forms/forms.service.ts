import { Injectable } from "@angular/core";

@Injectable()
export class FormsService {

    constructor() {}

    getClassProperty(form, property){
        if(!form || !form.controls[property]) return;

        if(form.controls[property].invalid && (form.controls[property].dirty || form.controls[property].touched)) return 'has-error';
        if(form.controls[property].valid && (form.controls[property].dirty || form.controls[property].touched)) return 'has-success';
    }

    hasErrors(form, property){
        if(!form || !form.controls[property]) return;

        if(form.controls[property].invalid && (form.controls[property].dirty || form.controls[property].touched)){
            return form.controls[property].errors;
        }

        return false;
    }

    hasError(form, property, validation){
        if(!form || !form.controls[property]) return;

        return form.controls[property].errors[validation];
    }

    canSave(form){
        if(form.valid) return true;

        return false;
    }
}
