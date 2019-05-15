export class MarginTracking {

    public PercentageExpected : number;
    public PercentageRealToDate : number;
    public PercentageToEnd : number;
    public ExpectedSales : number;
    public TotalExpensesExpected : number;
    public SalesOnMonth : number;
    public TotalExpensesOnMonth : number;
    public SalesAccumulatedToDate : number;
    public TotalExpensesAccumulatedToDate : number;
    public SalesRemainigToDate : number;
    public TotalExpensesRemainigToDate : number;
    public TotalSalesToEnd : number;
    public TotalExpensesToEnd : number;
    public Month : number;
    public Year : number;

    constructor(){
        this.PercentageExpected = 0;
        this.PercentageRealToDate = 0;
        this.PercentageToEnd = 0;
        this.ExpectedSales = 0;
        this.TotalExpensesExpected = 0;
        this.SalesOnMonth = 0;
        this.TotalExpensesOnMonth = 0;
        this.SalesAccumulatedToDate = 0;
        this.TotalExpensesAccumulatedToDate = 0;
        this.SalesRemainigToDate = 0;
        this.TotalExpensesRemainigToDate = 0;
        this.TotalSalesToEnd = 0;
        this.TotalExpensesToEnd = 0;
    }
}