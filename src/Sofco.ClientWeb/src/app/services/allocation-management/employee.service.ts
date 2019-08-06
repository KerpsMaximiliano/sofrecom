import { Injectable } from '@angular/core';
import { Response } from '@angular/http';
import { Service } from "../common/service";
import { HttpClient } from '@angular/common/http';
import {map} from 'rxjs/operators';

@Injectable()
export class EmployeeService {
  

  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getAll() {
    return this.http.get<any>(`${this.baseUrl}/employees`);
  }

  getManagers() {
    return this.http.get<any>(`${this.baseUrl}/users/managers`);
  }

  getOptions() {
    return this.http.get<any>(`${this.baseUrl}/employees/options`);
  }

  getAllForWorkTimeReport() {
    return this.http.get<any>(`${this.baseUrl}/employees/worktimeReport`);
  }

  getListItems() {
    return this.http.get<any>(`${this.baseUrl}/employees/listItems`);
  }

  getAnalytics(id){
    return this.http.get<any>(`${this.baseUrl}/employees/${id}/analytics`);
  }

  getById(id) {
    return this.http.get<any>(`${this.baseUrl}/employees/${id}`);
  }

  getInfo(id) {
    return this.http.get<any>(`${this.baseUrl}/employees/${id}/info`);
  }

  getProfile(id) {
    return this.http.get<any>(`${this.baseUrl}/employees/${id}/profile`);
  }

  search(model) {
    return this.http.post<any>(`${this.baseUrl}/employees/search`, model);
  }

  searchUnemployees(model) {
    return this.http.post<any>(`${this.baseUrl}/employees/search/unemployees`, model);
  }

  sendUnsubscribeNotification(model){
    return this.http.post<any>(`${this.baseUrl}/employees/sendUnsubscribeNotification/`, model);
  }

  finalizeExtraHolidays(id){
    return this.http.put<any>(`${this.baseUrl}/employees/${id}/finalizeExtraHolidays`, {});
  }

  addCategories(json){
    return this.http.put<any>(`${this.baseUrl}/employees/categories`, json);
  }

  getTasks(id){
    return this.http.get<any>(`${this.baseUrl}/employees/${id}/categories`);
  }

  put(id, json){
    return this.http.put<any>(`${this.baseUrl}/employees/${id}`, json);
  }

  getCurrentCategories() {
    return this.http.get<any>(`${this.baseUrl}/employees/currentCategories`);
  }

  getPendingHours(id) {
    return this.http.get<any>(`${this.baseUrl}/employees/${id}/pendingWorkingHours`);
  }

  addExternal(model){
    return this.http.post<any>(`${this.baseUrl}/employees/external`, model);
  }

  getEmployeesOptionByCurrentManager(){
    return this.http.get<any>(`${this.baseUrl}/employees/currentManager/options`);
  }

  getEmployeeEndNotification(model){
    return this.http.post<any>(`${this.baseUrl}/employees/endNotifications`, model);
  }

  getAdvancements(id){
    return this.http.get<any>(`${this.baseUrl}/employees/${id}/advancements`);
  }

  getRefunds(id){
    return this.http.get<any>(`${this.baseUrl}/employees/${id}/refunds`);
  }

  getCurrentAccount(id){
    return this.http.get<any>(`${this.baseUrl}/employees/${id}/currentAccount`);
  }

  getByEmail(email) {
    return this.http.get<any>(`${this.baseUrl}/employees/${email}/infoByMail`);
  }

  getUrlForImportFile(yearId: number, monthId: number, prepaidId: number){
    return `${this.baseUrl}/prepaid/${prepaidId}/import/${yearId}/${monthId}`;
  }

  getReport(){
    return this.http.get(`${this.baseUrl}/employees/report`, {
      responseType: 'arraybuffer',
      observe: 'response'
    }).pipe(map((res: any) => {
      return new Blob([res.body], { type: 'application/octet-stream' });
    }));
  }

  updateComments(json){
    return this.http.put<any>(`${this.baseUrl}/employees/updateAssingComment`, json);
  }
}
