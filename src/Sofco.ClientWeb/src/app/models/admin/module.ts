import { Menu } from "./menu";
import { Functionality } from "./functionality";
import { Role } from "./role";

export class Module {
    public id: number;
    public code: string;
    public description: string;
    public active: boolean;
    public menuId: number;
    public menu: Menu;
    public functionalities: Functionality[] = null;
    public available: boolean = false;

    constructor(){}
}