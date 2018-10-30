export class PurchaseOrderBalanceModel {
    public id: number;
    public purchaseOrderId: number;
    public number: string;
    public clientExternalName: string;
    public currencyId: number;
    public currencyText: string;
    public statusId: number;
    public statusText: string;
    public balance = 0;
    public ammount = 0;
    public receptionDate: Date;
    public accountManagerNames: string;
    public projectManagerNames: string;
    public details: any[] = [];
    public isAdjustment = false;

    constructor(){}
}