import { Group } from './group';

export interface Role{
    id: number,
    description: string,
    active: boolean,
    userGroups: Group[],
    startDate: Date,
    endDate: Date
}