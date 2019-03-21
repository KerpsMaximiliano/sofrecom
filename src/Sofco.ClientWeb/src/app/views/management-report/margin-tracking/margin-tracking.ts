import { Component, OnDestroy, OnInit } from "@angular/core";
import { MenuService } from "app/services/admin/menu.service";

@Component({
    selector: 'margin-tracking',
    templateUrl: './margin-tracking.html',
    styleUrls: ['./margin-tracking.scss']
})
export class MarginTrackingComponent implements OnInit, OnDestroy {

    isManager: boolean = false;
    isCdgOrDirector: boolean = false;

    constructor(private menuService: MenuService){}

    ngOnInit(): void {
        this.isManager = this.menuService.userIsManager;
        this.isCdgOrDirector = this.menuService.userIsManager || this.menuService.userIsCdg;
    }

    ngOnDestroy(): void {
        
    }
}