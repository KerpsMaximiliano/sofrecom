export class ResourceBillingItem {

    public id: number;
    public profileId: number;
    public seniorityId: number;
    public purchaseOrderId: number;
    public monthHour: number;
    public quantity: number;
    public amount: number;
    public subTotal: number;
    public deleted: boolean;

    constructor(data){
        
        if(data){
            this.profileId = data.profileId;
            this.seniorityId = data.seniorityId;
            this.purchaseOrderId = data.purchaseOrderId;
            this.monthHour = data.monthHour;
            this.quantity = data.quantity;
            this.amount = data.amount;
            this.subTotal = data.subTotal;
            this.deleted = false;
            this.id = data.id;
        }
        else{
            this.id = 0;
            this.subTotal = 0;
        }
    }
}