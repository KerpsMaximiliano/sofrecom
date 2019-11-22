import {map} from 'rxjs/operators';
import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Service } from "app/services/common/service";
import * as excel from 'exceljs';
import * as fs from 'file-saver';

@Injectable()
export class ManagementReportService {
  
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getDetail(serviceId) {
    return this.http.get<any>(`${this.baseUrl}/managementReport/${serviceId}`);
  }

  getBilling(serviceId) {
    return this.http.get<any>(`${this.baseUrl}/managementReport/${serviceId}/billing`);
  }

  getCostDetail(serviceId){
    return this.http.get<any>(`${this.baseUrl}/managementReport/${serviceId}/costDetail`);
  }  

  PostCostDetail(model){
    return this.http.post<any>(`${this.baseUrl}/managementReport/costDetail`, model);
  }
 
  PostCostDetailMonth(serviceId, model){
    return this.http.post<any>(`${this.baseUrl}/managementReport/${serviceId}/costDetailMonth`, model);
  }

  getCostDetailMonth(serviceId, month, year){
    return this.http.get<any>(`${this.baseUrl}/managementReport/${serviceId}/costDetailMonth/${month}/${year}`);
  }

  deleteContracted(contractedId){
    return this.http.delete<any>(`${this.baseUrl}/managementReport/${contractedId}/contrated`)
  }

  updateDates(id, model){
    return this.http.put<any>(`${this.baseUrl}/managementReport/${id}/dates`, model);
  }

  updateBilling(json) {
    return this.http.put<any>(`${this.baseUrl}/managementReportBillings`, json);
  }
  
  updateBillingData(json) {
    return this.http.put<any>(`${this.baseUrl}/managementReportBillings/data`, json);
  }

   //getOtherResources(){
  //   return this.http.get<any>(`${this.baseUrl}/managementReport/otherResources`)
   //}

  deleteOtherResources(id){
    return this.http.delete<any>(`${this.baseUrl}/managementReport/${id}/otherResources`)
  }

  GetOtherByMonth(idType, idCostDetail){
    return this.http.get<any>(`${this.baseUrl}/managementReport/${idType}/otherResources/${idCostDetail}`)
  }

  ExportTracing(model){
    return this.http.post(`${this.baseUrl}/managementReport/tracingReport`, model, {
      responseType: 'arraybuffer',
      observe: 'response'
    }).pipe(map((res: any) => {
      return new Blob([res.body], { type: 'application/octet-stream' });
    }));
  }

  updateQuantityResources(idBilling, quantityResources) {
    return this.http.put<any>(`${this.baseUrl}/managementReportBillings/${idBilling}/billedResources`, quantityResources);
  }

  send(model){
    return this.http.put<any>(`${this.baseUrl}/managementReport/send`, model);
  }

  close(model){
    return this.http.put<any>(`${this.baseUrl}/managementReport/close`, model);
  }

  addComment(model){
    return this.http.post<any>(`${this.baseUrl}/managementReport/comments`, model);
  }

  getComments(id){
    return this.http.get<any>(`${this.baseUrl}/managementReport/${id}/comments`);
  }

  deleteProfile(guid){
    return this.http.delete<any>(`${this.baseUrl}/managementReport/${guid}/profile`)
  }

  saveResources(id, json) {
    return this.http.post<any>(`${this.baseUrl}/managementReportBillings/${id}/resources`, json);
  }

  getResources(id, hitoId) {
    return this.http.get<any>(`${this.baseUrl}/managementReportBillings/${id}/${hitoId}/resources`);
  }
}