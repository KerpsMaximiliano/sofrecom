
import { User } from "models/admin/user";
import { Role } from "models/admin/role";
import { Module } from "models/admin/module";

export interface UserDetail extends User {
    roles: Role[],
    modules: Module[]
}