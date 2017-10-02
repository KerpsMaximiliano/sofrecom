export class HitoDetail {
    public id: number = 0;
    public total: number = 0;
    public descriptionOld: string;
    public unitPriceOld: number;

    constructor(
        public description: string,
        public quantity: number,
        public unitPrice: number,
        public externalProjectId: string,
        public externalHitoId: string,
        public currency: string
    ){
        this.descriptionOld = this.description;
        this.unitPriceOld = this.unitPriceOld;
    }
}