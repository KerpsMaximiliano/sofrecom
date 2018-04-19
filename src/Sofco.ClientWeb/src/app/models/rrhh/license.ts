import { LicenseStatus } from "../enums/licenseStatus";

export class License {
    public id: number;
    public employeeId: string;
    public managerId: number;
    public sectorId: number;
    public startDate: Date;
    public endDate: Date;
    public typeId: number;
    public withPayment: boolean;
    public daysQuantity: number;
    public hasCertificate: boolean;
    public parcial: boolean;
    public final: boolean;
    public comments: string;
    public examDescription: string;
    public status: LicenseStatus;

    constructor(){
        this.withPayment = true;
    }
}