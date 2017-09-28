export class HitoDetail {
    public id: number = 0;
    public total: number = 0;
    
    constructor(
        public description: string,
        public quantity: number,
        public unitPrice: number,
        public externalProjectId: string,
        public externalHitoId: string,
        public currency: string
    ){}
}