import { LicenseStatus } from "../enums/licenseStatus";

export class License {
    public id: number;
    public employeeId: string;
    public managerId: string;
    public sectorId: string;
    public startDate: Date;
    public endDate: Date;
    public typeId: string;
    public withPayment: boolean;
    public daysQuantity: number;
    public hasCertificate: boolean;
    public parcial: boolean;
    public final: boolean;
    public comments: string;
    public examDescription: string;
    public status: LicenseStatus;

    constructor(){
        this.startDate = new Date();
        this.endDate = new Date();
        this.withPayment = true;
    }
}