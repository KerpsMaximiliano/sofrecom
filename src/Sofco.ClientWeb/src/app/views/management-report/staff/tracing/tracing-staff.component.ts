import { Component, OnDestroy, OnInit } from "@angular/core";
// import { Worksheet } from "exceljs";

@Component({
    selector: 'tracing-staff',
    templateUrl: './tracing-staff.component.html',
    styleUrls: ['./tracing-staff.component.scss']
})

export class TracingStaffComponent implements OnInit, OnDestroy {
   
    months: any[] = new Array()
    accumulatedMonths: any[] = new Array()

    ngOnInit(): void {
      
    }

    ngOnDestroy(): void {
    }
   

    calculateAccumulated(months){
        this.months = months
        this.accumulatedMonths = new Array()

        months.forEach((month, index) => {
            let acomulatedBudget = 0
            let acomulatedPfa1 = 0
            let acomulatedPfa2 = 0
            let acomulatedReal = 0

            for (let i = 0; i <= index; i++) {

                acomulatedBudget += months[i].budget.totalCost
                acomulatedPfa1 += months[i].pfa1.totalCost
                acomulatedPfa2 += months[i].pfa2.totalCost
                acomulatedReal += months[i].real.totalCost
            }

            var acomulatedMont = {
                display: month.display,
                monthYear: month.monthYear,
                totalBudget: month.budget.totalCost,
                totalPfa1: month.pfa1.totalCost,
                totalPfa2: month.pfa2.totalCost,
                totalReal: month.real.totalCost,
                acomulatedBudget: acomulatedBudget,
                acomulatedPfa1: acomulatedPfa1,
                acomulatedPfa2: acomulatedPfa2,
                acomulatedReal: acomulatedReal,
            }

            this.accumulatedMonths.push(acomulatedMont)
        })
    }

    // createWorksheet(workbook){
    //     let worksheet: Worksheet = workbook.addWorksheet('Seguimiento');

    //     var row1 = ["", "Mensual", "", "", "", "", "", "", "Acumulado", "", "", "", "", "", ""];
    //     var row2 = ["", "BUDGET", "PFA1", "PFA2", "REAL", "Desvio PRES", "Desvio PFA1", "Desvio PFA2", "BUDGET", "PFA1", "PFA2", "REAL", "Desvio PRES", "Desvio PFA1", "Desvio PFA2"];

    //     worksheet.addRows([row1, row2]);

    //     var i = 3;
    //     this.accumulatedMonths.forEach(x => {
    //         var rowToAdd = [x.display, x.totalBudget, x.totalPfa1, x.totalPfa2, x.totalReal, "", "", "", x.acomulatedBudget, x.acomulatedPfa1, x.acomulatedPfa2, x.acomulatedReal];
    //         worksheet.addRow(rowToAdd);

    //         var row = worksheet.getRow(i);

    //         row.eachCell(cell => {
    //             if(cell.address.includes("F") || cell.address.includes("G") || cell.address.includes("H") || 
    //                cell.address.includes("M") || cell.address.includes("N") || cell.address.includes("O")){
    //                    return;
    //                }

    //             cell.numFmt = '#,##0.00';
    //         });

    //         i++
    //     });

    //     worksheet.mergeCells("B1:H1");
    //     worksheet.mergeCells("I1:O1");
    //     worksheet.getCell("B1").alignment = { horizontal:'center'};
    //     worksheet.getCell("I1").alignment = { horizontal:'center'};

    //     worksheet.getCell("B2").alignment = { horizontal:'center'};
    //     worksheet.getCell("C2").alignment = { horizontal:'center'};
    //     worksheet.getCell("D2").alignment = { horizontal:'center'};
    //     worksheet.getCell("E2").alignment = { horizontal:'center'};
    //     worksheet.getCell("F2").alignment = { horizontal:'center'};
    //     worksheet.getCell("G2").alignment = { horizontal:'center'};
    //     worksheet.getCell("H2").alignment = { horizontal:'center'};
    //     worksheet.getCell("I2").alignment = { horizontal:'center'};
    //     worksheet.getCell("J2").alignment = { horizontal:'center'};
    //     worksheet.getCell("K2").alignment = { horizontal:'center'};
    //     worksheet.getCell("L2").alignment = { horizontal:'center'};
    //     worksheet.getCell("M2").alignment = { horizontal:'center'};
    //     worksheet.getCell("N2").alignment = { horizontal:'center'};
    //     worksheet.getCell("O2").alignment = { horizontal:'center'};

    //     for(var i = 1; i < 16; i++){
    //         worksheet.getColumn(i).width = 15;
    //     }

    //     var firstRow = worksheet.getRow(1);
    //     var secondRow = worksheet.getRow(2);

    //     firstRow.eachCell(cell => {
    //         this.drawBorder(cell, 'bottom');
    //         this.drawBorder(cell, 'right');
    //     });
        
    //     secondRow.eachCell(cell => {
    //         this.drawBorder(cell, 'bottom');
    //     });

    //     var column1 = worksheet.getColumn(1);
    //     column1.eachCell(cell => {
    //         this.drawBorder(cell, 'right');
    //     });

    //     var column5 = worksheet.getColumn(5);
    //     column5.eachCell(cell => {
    //         this.drawBorder(cell, 'right');
    //     });

    //     var column8 = worksheet.getColumn(8);
    //     column8.eachCell(cell => {
    //         this.drawBorder(cell, 'right');
    //     });

    //     var column12 = worksheet.getColumn(12);
    //     column12.eachCell(cell => {
    //         this.drawBorder(cell, 'right');
    //     });

    //     var column15 = worksheet.getColumn(15);
    //     column15.eachCell(cell => {
    //         this.drawBorder(cell, 'right');
    //     });

    //     var lastRow = worksheet.getRow(worksheet.rowCount);

    //     lastRow.eachCell(cell => {
    //         this.drawBorder(cell, 'bottom');
    //     });
    // }

    // private drawBorder(cell, position) {
    //     const borderBlack = "FF000000";

    //     if (cell.border) {
    //         if (cell.border[position]) {
    //             cell.border[position].style = 'thin';
    //             cell.border[position].color.argb = borderBlack;
    //         }
    //         else {
    //             cell.border[position] = { style: 'thin', color: { argb: borderBlack } };
    //         }
    //     }
    //     else {
    //         cell.border = {};
    //         cell.border[`${position}`] = {};
    //         cell.border[position] = { style: 'thin', color: { argb: borderBlack } };
    //     }
    // }
}