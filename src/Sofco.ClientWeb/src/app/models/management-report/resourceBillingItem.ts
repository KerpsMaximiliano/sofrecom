export class ResourceBillingItem {

    public profileId: number;
    public seniorityId: number;
    public purchaseOrderId: number;
    public monthHour: number;
    public quantity: number;
    public amount: number;
    public subTotal: number;

    constructor(data){
        
        if(data){
            this.profileId = data.profileId;
            this.seniorityId = data.seniorityId;
            this.purchaseOrderId = data.purchaseOrderId;
            this.monthHour = data.monthHour;
            this.quantity = data.quantity;
            this.amount = data.amount;
            this.subTotal = data.subTotal;
        }
    }
}