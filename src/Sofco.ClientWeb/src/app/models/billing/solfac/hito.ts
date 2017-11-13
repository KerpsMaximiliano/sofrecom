export class Hito {

    constructor(
        public id: number,
        public description: string,
        public total: number,
        public externalProjectId: string,
        public externalHitoId: string,
        public currency: string,
        public month: number,
        public solfacId: number
    ){}
}