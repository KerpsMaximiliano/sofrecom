import { Injectable } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Service } from "app/services/common/service";

@Injectable()
export class WorkflowService {
  private baseUrl: string;

  constructor(private http: HttpClient, private service: Service) {
    this.baseUrl = this.service.UrlApi;
  }

  getTransitions(model) {
    return this.http.post<any>(`${this.baseUrl}/${model.entityController}/possibleTransitions`, model);
  }

  post(model) {
    return this.http.post<any>(`${this.baseUrl}/${model.entityController}/transition`, model);
  }

  doMassiveTransitions(model) {
    return this.http.post<any>(`${this.baseUrl}/workflows/transition`, model);
  }

  put(model) {
    return this.http.put<any>(`${this.baseUrl}/${model.entityController}/transition`, model);
  }

  getWorkflows() {
    return this.http.get<any>(`${this.baseUrl}/workflows`);
  }

  getWorkflowById(id) {
    return this.http.get<any>(`${this.baseUrl}/workflows/${id}`);
  }

  getTypes() {
    return this.http.get<any>(`${this.baseUrl}/workflows/types`);
  }

  add(model) {
    return this.http.post<any>(`${this.baseUrl}/workflows`, model);
  }

  update(model) {
    return this.http.put<any>(`${this.baseUrl}/workflows/${model.id}`, model);
  }

  getStates() {
    return this.http.get<any>(`${this.baseUrl}/workflows/states`);
  }

  addTransition(model) {
    return this.http.post<any>(`${this.baseUrl}/workflowtransitions`, model);
  }

  putTransition(model) {
    return this.http.put<any>(`${this.baseUrl}/workflowtransitions`, model);
  }

  deleteTransition(id) {
    return this.http.delete<any>(`${this.baseUrl}/workflowtransitions/${id}`);
  }

  getTransition(id) {
    return this.http.get<any>(`${this.baseUrl}/workflowtransitions/${id}`);
  }

  //Workflow States
  getWorkflowStates() {
    return this.http.get<any>(`${this.baseUrl}/workflowstate`);
  }

  getWorkflowStateType() {
    return this.http.get<any>(`${this.baseUrl}/workflowstate/types`);
  }

  getWorkflowState(id: number) {
    return this.http.get<any>(`${this.baseUrl}/workflowstate/${id}`);
  }

  activeWorkflowState(id, active) {
    return this.http.put<any>(`${this.baseUrl}/workflowstate/${id}/active/${active}`, {});
  }

  editWorkflowState(model) {
    return this.http.put<any>(`${this.baseUrl}/workflowstate`, model);
  }

  AddWorkflowState(model) {
    return this.http.post<any>(`${this.baseUrl}/workflowstate`, model);
  }

}