
import { Group } from "models/admin/group";

export interface User {
    id: number,
    name: string,
    userName: string,
    email: string,
    active: boolean,
    startDate: Date,
    endDate: Date,
    groups: Group[]
}