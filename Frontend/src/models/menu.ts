export class Menu{
    constructor(
        public id: number,
        public code: string = "",
        public description: string = "",
        public active: boolean = true,
        public url: string = ''
    ){}
    
}