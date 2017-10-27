export class AllocationModel {
    public allocations: Allocation[];
    public monthsHeader: string[];
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
}