export class ResourceBillingItem {

    public id: number;
    public profile: string;
    public seniorityId: number;
    public employeeId: number;
    public monthHour: number;
    public quantity: number;
    public amount: number;
    public subTotal: number;
    public deleted: boolean;

    constructor(data){
        
        if(data){
            this.profile = data.profile;
            this.seniorityId = data.seniorityId;
            this.employeeId = data.employeeId;
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