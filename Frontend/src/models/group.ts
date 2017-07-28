import { Role } from 'models/role';


export interface Group {
    id: number,
    description: string,
    active: boolean
    role: Role,
    startDate: Date,
    endDate: Date
}

/*
export class Group{

    constructor(
        public role: Role,
        public id: number = 0,
        public description: string = "",
        public active: boolean = false,
        public startDate: Date = null,
        public endDate: Date = null
    ){}
}
*/