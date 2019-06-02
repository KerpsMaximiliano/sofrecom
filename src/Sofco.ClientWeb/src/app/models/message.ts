export class Message {
    text: string;

    constructor(
        public folder: string,
        public code: string,
        public type: number,
        public translate: boolean
    ){}
}