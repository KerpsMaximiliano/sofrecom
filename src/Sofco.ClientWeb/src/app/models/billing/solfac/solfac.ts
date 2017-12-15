import { HitoDetail } from 'app/models/billing/solfac/hitoDetail';
import { Hito } from 'app/models/billing/solfac/hito';

export class Solfac {
    constructor(
        public businessName: string,
        public clientName: string,
        public celphone: string,
        public statusName: string,
        public statusId: string,
        public contractNumber: string,
        public project: string,
        public documentType: number,
        public userApplicantId: number,
        public imputationNumber1: string,
        public imputationNumber3: number,
        public currencyId: number,
        public totalAmount: number,
        public withTax: boolean,
        public capitalPercentage: number,
        public buenosAiresPercentage: number,
        public otherProvince1Percentage: number,
        public province1Id: number,
        public otherProvince2Percentage: number,
        public province2Id: number,
        public otherProvince3Percentage: number,
        public province3Id: number,
        public hitos: Hito[],
        public details: HitoDetail[],
        public attachedParts: string,
        public particularSteps: string,
        public paymentTermId: number,
        public userApplicantName: string,
        public projectId: string,
        public invoicesId: string[],
        public customerId: string,
        public serviceId: string,
        public service: string,
        public analytic: string,
        public remito: boolean
    ){}
}