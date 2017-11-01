export class AllocationModel {
    public allocations: Allocation[];
    public monthsHeader: MonthHeader[];
}

export class Allocation {
    public months: AllocationMonth[];
    public analyticId: number;
    public analyticTitle: string;
    public employeeId: number;
}

export class AllocationMonth {
    public allocationId: number;
    public date: Date;
    public percentage: number;
    public updated: boolean;
}

export class MonthHeader {
    public display: string;
    public month: number;
    public year: number;
}