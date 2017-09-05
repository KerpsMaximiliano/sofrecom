import { Module } from "models/admin/module";

export class Functionality {
    constructor(
        public id: number,
        public code: string,
        public description: string,
        public active: boolean,
        public startDate: Date,
        public endDate: Date,
        public modules: Module[]
    ){}

}