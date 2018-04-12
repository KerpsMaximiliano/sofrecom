import { LicenseStatus } from "../enums/licenseStatus";
import { Option } from "../option";

export class LicenseDetail {
    public id: number;
    public employeeId: number;
    public employeeName: string;
    public managerId: number;
    public managerName: string;
    public sectorId: number;
    public sectorName: string;
    public startDate: Date;
    public endDate: Date;
    public typeId: number;
    public typeName: string;
    public withPayment: boolean;
    public daysQuantity: number;
    public hasCertificate: boolean;
    public parcial: boolean;
    public final: boolean;
    public comments: string;
    public examDescription: string;
    public status: LicenseStatus;
    public statusName: string;

    public files: Array<any> = new Array<any>();

    constructor(){
    }
}