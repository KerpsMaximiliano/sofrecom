export class Invoice {
    public details: Detail[] = new Array<Detail>();
    public accountName: string;
    public address: string;
    public zipcode: string;
    public city: string;
    public province: string;
    public country: string;
    public cuit: string;
    public service: string;
    public project: string;
    public projectId: string;
    public analytic: string;

    constructor(){}
}

export class Detail {
    constructor(
        public description: string,
        public quantity: number,
    ){}
}