import { Group } from './group';

export interface User {
    id: number,
    email: string,
    active: boolean,
    startDate: Date,
    endDate: Date,
    groups: Group[]
}