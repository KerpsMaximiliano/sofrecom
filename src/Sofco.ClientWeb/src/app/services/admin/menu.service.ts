import { Injectable } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { Service } from '../common/service';
import { Menu } from '../../models/admin/menu';
import { HttpClient } from '@angular/common/http';
import { UserInfoService } from '../common/user-info.service';

@Injectable()
export class MenuService {
    private baseUrl: string;

    public menu: Menu[];
    public userIsDirector: boolean;
    public userIsManager: boolean;
    public userIsDaf: boolean;
    public userIsGaf: boolean;
    public userIsCdg: boolean;
    public userIsRrhh: boolean;
    public isManagementReportDelegate: boolean;
    public userIsCompliance: boolean;
    public currentUser: any;
    public user: any;

    public dafMail: string;
    public cdgMail: string;
    public pmoMail: string;
    public rrhhMail: string;
    public sellerMail: string;
    public areaIds: any[];
    public sectorIds: any[];
    public refundDelegates: any[];

    constructor(private http: HttpClient, private service: Service) {
        this.baseUrl = this.service.UrlApi;

        if (!this.menu) {
            const menu = JSON.parse(localStorage.getItem('menu'));
            if (menu != null) {
                this.menu = menu.menus;
                this.userIsDirector = menu.isDirector;
                this.userIsManager = menu.isManager;
                this.isManagementReportDelegate = menu.isManagementReportDelegate;
                this.userIsDaf = menu.isDaf;
                this.userIsGaf = menu.isGaf;
                this.userIsCdg = menu.isCdg;
                this.userIsRrhh = menu.isRrhh;
                this.userIsCompliance = menu.isCompliance;

                this.dafMail = menu.dafMail;
                this.cdgMail = menu.cdgMail;
                this.pmoMail = menu.pmoMail;
                this.rrhhMail = menu.rrhhMail;
                this.sellerMail = menu.sellerMail;
                this.areaIds = menu.areaIds;
                this.sectorIds = menu.sectorIds;

                this.refundDelegates = menu.refundDelegates;
            }
        }

        if (!this.currentUser) {
            this.currentUser = Cookie.get('currentUser');
        }

        const userInfo = UserInfoService.getUserInfo();

        if (userInfo) {
            this.currentUser = userInfo.name;
            this.user = userInfo;
        }
    }

    get() {
       return this.http.get<any>(`${this.baseUrl}/menu/`);
    }

    hasModule(module: string) {
        return this.menu.findIndex(x => x.module === module) > -1;
    }

    hasFunctionality(module: string, functionality: string) {
        return this.menu.findIndex(x => x.module === module && x.functionality === functionality) > -1;
    }

    hasAdminMenu() {
       if (this.hasModule("USR") || this.hasModule("GRP") || this.hasModule("ROL") || this.hasModule("MOD") || this.hasModule("FUNC")){
            return true;
       }
       return false;
    }

    hasBillingMenu() {
        if (this.hasModule("SOLFA") || this.hasModule("PUROR")) {
            return true;
       }
       return false;
    }

    hasAllocationManagementMenu() {
        if (this.hasModule("ALLOC")) {
            return true;
        }
        return false;
    }

    hasAdvancementMenu() {
        if (this.hasModule("ADVAN")) {
            return true;
        }
        return false;
    }

    hasContractsMenu() {
        if (this.hasModule("CONTR")) {
            return true;
        }
        return false;
    }

    hasReportMenu() {
        if (this.hasModule("REPOR")) {
             return true;
        }
        return false;
    }

    hasWorkTimeManagement() {
        if (this.hasModule("WOTIM")) {
            return true;
       }
       return false;
    }

    hasAreaAccess(areaId:number):boolean {
        return this.areaIds.indexOf(areaId)>-1;
    }

    hasSectorAccess(sectorIds:number[]):boolean {
        let isValid = false;
        this.sectorIds.forEach(x => {
            if(sectorIds.indexOf(x)>-1){
                isValid = true;
            }
        });
        return isValid;
    }

    hasSectors():boolean {
        return this.sectorIds.length > 0;
    }
}
