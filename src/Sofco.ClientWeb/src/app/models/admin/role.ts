import { Group } from "app/models/admin/group";
import { Module } from "app/models/admin/module";

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