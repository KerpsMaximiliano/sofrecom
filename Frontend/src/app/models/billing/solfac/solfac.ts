import { HitoDetail } from 'app/models/billing/solfac/hitoDetail';

export class Solfac {
    constructor(
        public businessName: string,
        public clientName: string,
        public celphone: string,
        public statusName: string,
        public statusId: number,
        public contractNumber: string,
        public project: string,
        public documentTypeId: number,
        public userApplicantId: number,
        public imputationNumber1: string,
        public imputationNumber3: number,
        public amount: number,
        public iva21: number,
        public currencyId: number,
        public totalAmount: number,
        public capitalPercentage: number,
        public buenosAiresPercentage: number,
        public otherProvince1Percentage: number,
        public province1Id: number,
        public otherProvince2Percentage: number,
        public province2Id: number,
        public otherProvince3Percentage: number,
        public province3Id: number,
        public hitos: HitoDetail[],
        public attachedParts: string,
        public particularSteps: string,
        public timeLimit: string,
        public userApplicantName: string,
        public projectId: string,
        public invoiceId: string
    ){}
}