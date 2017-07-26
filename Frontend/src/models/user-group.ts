import { Role } from 'models/role';

export interface UserGroup{
    id: number,
    description: string,
    active: boolean
    rol: Role,
    startDate: Date,
    endDate: Date
}