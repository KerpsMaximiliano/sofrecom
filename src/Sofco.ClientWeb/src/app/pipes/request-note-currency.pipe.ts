import { Pipe, PipeTransform } from "@angular/core";

@Pipe({
    name: 'requestNoteCurrency',
    pure: true
})

export class RequestNoteCurrencyPipe implements PipeTransform {
    private currencies: Array<{id: number, description: string}> = [
        {id: 1, description: '$'}, {id:2, description: 'u$s'}
    ];

    transform(currencyId: number): string{
        let find = this.currencies.find(currency => currency.id == currencyId);
        if(find != undefined) {
            return find.description;
        } else {
            return ""
        }
    }
}