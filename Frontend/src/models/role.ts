import { Group } from './group';
import { Functionality } from './functionality';

export interface Role{
    id: number,
    description: string,
    active: boolean,
    groups: Group[],
    functionalities: Functionality[],
    startDate: Date,
    endDate: Date
}