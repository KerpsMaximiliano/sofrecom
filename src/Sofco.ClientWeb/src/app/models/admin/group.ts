
import { Role } from "app/models/admin/role";

export class Group {
    public id: number;
    public description: string;
    public active: boolean;
    public role: Role;
    public startDate: Date;
    public  endDate: Date;
    public roleId: any;
    public email: string;

    constructor() {}
}