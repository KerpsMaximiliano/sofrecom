
import { User } from "./user";
import { Role } from "./role";
import { Module } from "./module";

export interface UserDetail extends User {
    roles: Role[],
    modules: Module[]
}