import { UserGroup } from './user-group';

export interface Role{
    id: number,
    description: string,
    active: boolean,
    userGroups: UserGroup[],
    startDate: Date,
    endDate: Date
}