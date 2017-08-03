import { Functionality } from './functionality';
import { Role } from './role';
import { User } from 'models/user';
export interface UserDetail extends User{
    roles: Role[],
    Functionalities: Functionality[]
}