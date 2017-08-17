import { Menu } from './menu';
import { Functionality } from 'models/functionality';
export class Module{
    constructor(
        public id: number,
        public code: string,
        public description: string,
        public active: boolean,
        public menuId: number,
        public menu: Menu,
        public functionalities: Functionality[] = null,
        public available: boolean = false
    ){}
}