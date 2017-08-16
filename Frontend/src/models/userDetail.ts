import { Module } from './module';
import { Role } from './role';
import { User } from 'models/user';

export interface UserDetail extends User{
    roles: Role[],
    modules: Module[]
}