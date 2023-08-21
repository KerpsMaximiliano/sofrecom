import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Service } from "../common/service";

@Injectable({
  providedIn: "root",
})
export class ProvidersAreaService {
  private url: string;
  private industry: any;
  private mode: boolean = false;

  constructor(private http: HttpClient, private service: Service) {
    this.url = this.service.UrlApi;
  }

  public getAll(): Observable<any> {
    return this.http.get<any>(`${this.url}/providersArea`);
  }

  public get(id: number): Observable<any> {
    return this.http.get<any>(`${this.url}/providersArea/${id}`);
  }

  public edit(id: number, industry: any) {
    return this.http.put<any>(`${this.url}/providersArea/${id}`, industry);
  }

  public post(industry: any) {
    return this.http.post<any>(`${this.url}/providersArea`, industry);
  }

  /**
   * El parametro 'action' determina:
   *   > true: rubro critico.
   *   > false: rubro no critico.
   * @param id ID del rubro.
   * @param industry Rubro.
   * @param critical Acción que se desea realizar.
   * @returns
   */
  public action(industry: any, action: boolean): any {
    let critical: string = action ? "Enable" : "Disable";
    return this.http.post<any>(
      `${this.url}/ProvidersArea/${critical}/${industry.id}`,
      industry
    );
  }

  public setIndustry(industry: any): void {
    if (industry) this.industry = industry;
  }

  public getIndustry(): any {
    return this.industry;
  }

  public setMode(mode: boolean): void {
    if (mode) this.mode = mode;
  }

  public getMode(): boolean {
    return this.mode;
  }
}
