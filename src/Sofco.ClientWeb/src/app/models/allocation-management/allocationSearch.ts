export class AllocationSearch {
    public percentage: number;
    public dateSince: Date;
    public dateTo: Date;

    constructor(){
        this.percentage = 0;
        this.dateSince = new Date();
        this.dateTo = new Date();
    }
}