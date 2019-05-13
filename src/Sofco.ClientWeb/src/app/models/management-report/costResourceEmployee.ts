import { MonthDetailCost } from "./monthDetailCost"

export class CostDetailEmployee {

    public EmployeeId : number
    public UserId : number
    public TypeName : string
    public Display : string
    public MontDetailCost : MonthDetailCost[]

    constructor(){
    }
}