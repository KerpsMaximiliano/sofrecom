
import {map} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Service } from "../common/service";
import { MenuService } from '../admin/menu.service';
import { HttpClient } from '@angular/common/http';

@Injectable()
export class SolfacService {
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service, private menuService: MenuService) {
    this.baseUrl = this.service.UrlApi;
  }

  getOptions(serviceId, opportunityNumber){
    return this.http.get<any>(`${this.baseUrl}/solfacs/options/${serviceId}/${opportunityNumber}`);
  }

  search(parameters) {
    return this.http.post<any>(`${this.baseUrl}/solfacs/search`, parameters);
  }

  get(solfacId) {
    return this.http.get<any>(`${this.baseUrl}/solfacs/${solfacId}`);
  }

  getStatus() {
    return this.http.get<any>(`${this.baseUrl}/solfacs/status`);
  }

  getHistories(solfacId) {
    return this.http.get<any>(`${this.baseUrl}/solfacs/${solfacId}/histories`);
  }

  getAttachments(solfacId) {
    return this.http.get<any>(`${this.baseUrl}/solfacs/${solfacId}/files`);
  }

  getCertificatesRelated(solfacId) {
    return this.http.get<any>(`${this.baseUrl}/solfacs/${solfacId}/certificates`);
  }

  add(model) {
     return this.http.post<any>(`${this.baseUrl}/solfacs`, model);
  }

  validate(model) {
    return this.http.post<any>(`${this.baseUrl}/solfacs/validate`, model);
  }

  update(model) {
    return this.http.put<any>(`${this.baseUrl}/solfacs`, model);
  }

  updateAndSend(model) {
    return this.http.put<any>(`${this.baseUrl}/solfacs/send`, model);
  }

  send(model) {
     return this.http.post(`${this.baseUrl}/solfacs/send`, model);
  }

  changeStatus(id, json) {
     json.userId = this.menuService.user.id;

     return this.http.post<any>(`${this.baseUrl}/solfacs/${id}/status`, json);
  }

  updateBill(id, json) {
    json.userId = this.menuService.user.id;

    return this.http.put<any>(`${this.baseUrl}/solfacs/${id}/bill`, json);
 }

 updateCash(id, json) {
    json.userId = this.menuService.user.id;

    return this.http.put<any>(`${this.baseUrl}/solfacs/${id}/cash`, json);
  }

  delete(id) {
     return this.http.delete<any>(`${this.baseUrl}/solfacs/${id}`);
  }

  deleteCertificate(id, certificateId) {
    return this.http.delete<any>(`${this.baseUrl}/solfacs/${id}/certificate/${certificateId}`);
  }

  deleteDetail(id) {
    return this.http.delete<any>(`${this.baseUrl}/solfacs/details/${id}`);
 }

  deleteInvoiceOfSolfac(id, invoiceId) {
    return this.http.delete<any>(`${this.baseUrl}/solfacs/${id}/invoice/${invoiceId}`);
  }

  deleteFile(id) {
    return this.http.delete<any>(`${this.baseUrl}/solfacs/file/${id}`);
  }

  getInvoices(id) {
    return this.http.get<any>(`${this.baseUrl}/solfacs/${id}/invoices`);
  }

  getUrlForImportFile(id){
    return `${this.baseUrl}/solfacs/${id}/file`;
  }

  addInvoices(id, invoices) {
    return this.http.post<any>(`${this.baseUrl}/solfacs/${id}/invoices`, invoices);
  }

  addCertificates(id, certificates) {
    return this.http.post<any>(`${this.baseUrl}/solfacs/${id}/certificates`, certificates);
  }

  downloadFile(id) {
    return this.http.get(`${this.baseUrl}/solfacs/file/${id}/download`, {
       responseType: 'arraybuffer',
       observe: 'response'
    }).pipe(map((res: any) => {
      return new Blob([res.body], { type: 'application/octet-stream' });
    }));
  }

  getFile(id) {
    return this.http.get<any>(`${this.baseUrl}/solfacs/file/${id}`);
  }
}
