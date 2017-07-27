import { Role } from 'models/role';

export interface Group {
    id: number,
    description: string,
    active: boolean
    rol: Role,
    startDate: Date,
    endDate: Date
}