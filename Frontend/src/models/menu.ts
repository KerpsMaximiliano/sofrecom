import { Module } from 'models/module';
export class Menu{
    constructor(
        public functionality: string = "",
        public description: string = "",
        public module: string = "",
    ){}
    
}