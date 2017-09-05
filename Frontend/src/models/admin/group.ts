
import { Role } from "models/admin/role";

export interface Group {
    id: number,
    description: string,
    active: boolean
    role: Role,
    startDate: Date,
    endDate: Date,
    roleId: number
}