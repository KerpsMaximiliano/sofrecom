
import { User } from "app/models/admin/user";
import { Role } from "app/models/admin/role";
import { Module } from "app/models/admin/module";

export interface UserDetail extends User {
    roles: Role[],
    modules: Module[]
}