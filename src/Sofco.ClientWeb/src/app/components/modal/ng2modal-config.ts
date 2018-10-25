export class Ng2ModalConfig{
    public deleteButton = false;
    public acceptInlineButton = false;
    public isDraggable = false;

    constructor(
        public title: string = "Modal Title",
        public id: string = "MyModal",
        public acceptButton: boolean = true,
        public cancelButton: boolean = true,
        public acceptButtonText: string = "Accept",
        public cancelButtonText: string = "Cancel",
        public closeIcon: boolean = true
    ){}
}