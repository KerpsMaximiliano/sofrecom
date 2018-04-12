
export class Option{
    public included: boolean;
    public index: number;

    constructor(
        public value: string,
        public text: string
    ){
        this.included = false;
    }
}