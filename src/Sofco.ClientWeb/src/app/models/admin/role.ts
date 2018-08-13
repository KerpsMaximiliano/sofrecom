import { Group } from "./group";
import { Module } from "./module";

export class Role {
    constructor(
        public id: number,
        public description: string = "",
        public active: boolean = true,
        public groups: Group[] = null,
        public startDate: Date = new Date(),
        public endDate: Date = null,
        public modules: Module[] = null,
    ){}
    
}