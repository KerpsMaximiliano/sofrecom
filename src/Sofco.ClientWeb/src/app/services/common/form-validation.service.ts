import { FormControl } from '@angular/forms';

export class FormValidationService {
    public static dateRangeValidator(control: FormControl) {
        const itemDate = control.value;
        const today = new Date();
        if(itemDate > today){
            return {
                invalidDate: itemDate
            };
        }
        return null;
    }
}
