import { Functionality } from 'models/functionality';
export class Module{
    constructor(
        public id: number,
        public code: string,
        public description: string,
        public active: boolean,
        public startDate: Date,
        public endDate: Date = null,
        public functionalities: Functionality[] = null
    ){}
}