export class DatatablesOptions{
    constructor(
        public b_edit: boolean = false,
        public b_delete: boolean = false,
        public b_view: boolean = false,
        public b_other1: boolean = false,
        public b_other2: boolean = false,
        public b_other3: boolean = false,
        public other1Icon: string = "",
        public other2Icon: string = "",
        public other3Icon: string = "",
        public orderByColumn: number = 0,
        public orderByAscDesc: string = "asc"){}

    
}