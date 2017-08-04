import { Group } from './group';
import { Functionality } from './functionality';

/*
export interface Role{
    id: number,
    description: string,
    active: boolean,
    groups: Group[],
    functionalities: Functionality[],
    startDate: Date,
    endDate: Date
}*/

export class Role{
    constructor(
        public id: number,
        public description: string = "",
        public active: boolean = true,
        public groups: Group[] = null,
        public functionalities: Functionality[] = null,
        public startDate: Date = new Date(),
        public endDate: Date = null
    ){}
    
}