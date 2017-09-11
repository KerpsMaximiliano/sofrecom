export class Invoice {
    public accountName: string;
    public address: string;
    public zipcode: string;
    public city: string;
    public province: string;
    public country: string;
    public cuit: string;
    public service: string;
    public project: string;
    public projectId: string;
    public analytic: string;
    public id: number = 0;
    public excelFileName: string;
    public pdfFileName: string;
    public invoiceStatus: string;
    public excelFileCreatedDate: string;
    public pdfFileCreatedDate: string;
    public createdDate: string;
    public invoiceNumber: string;

    constructor(){}
}