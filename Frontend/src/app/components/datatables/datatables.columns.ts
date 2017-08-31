import { DatatablesDataType } from "./datatables.datatype";
import { DatatablesAlignment } from "./datatables.alignment";

export class DatatablesColumn {

    constructor(
        public name: string,
        public title: string,
        public width: string = '0', //0 es no aplicar width. x ej: 30px o 20%
        public visibility: number = 1, //1 -> visible. 0 -> invisible
        public dataType: DatatablesDataType = 1, //string
        public alignment: DatatablesAlignment = 1 //left,
    ){}
}

