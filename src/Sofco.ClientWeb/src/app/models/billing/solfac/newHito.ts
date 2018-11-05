export class NewHito {
    public name: string;
    public ammount: number;
    public statusCode: number;
    public projectId: string;
    public opportunityId: string;
    public managerId: string;
    public externalHitoId: string;
    public moneyId: number;
    public month: number;
    public startDate: Date;
    public ammountFirstHito: number;

    constructor() {
        this.startDate = new Date();
    }
}