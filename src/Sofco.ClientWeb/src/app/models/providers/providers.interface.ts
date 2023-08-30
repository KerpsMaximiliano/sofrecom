export interface IProviders {
  id: number;
  name: string;
  userApplicantId: null;
  score: null;
  startDate: string;
  endDate: null;
  active: boolean;
  cuit: null | string;
  ingresosBrutos: number | null;
  condicionIVA: number | null;
  address: null | string;
  city: null | string;
  zipCode: null | string;
  province: null | string;
  contactName: null | string;
  phone: null | string;
  email: null | string;
  webSite: null | string;
  comments: null | string;
  country: null | string;
  providersAreaProviders: IProvidersAreaProvider[];
}

export interface IProvidersAreaProvider {
  id: number;
  providerAreaId: number;
  providerId: number;
}

export interface IProvidersArea {
  id: number;
  description: string;
  critical: boolean;
  active: boolean;
  rnAmmountReq: boolean;
  startDate: string;
  endDate: string;
}
