export class HitoDetail {

    constructor(
        public id: number,
        public description: string,
        public total: number,
        public quantity: number,
        public unitPrice: number,
        public hitoId: number,
        public externalHitoId: string
    ){}
}