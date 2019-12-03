import { Injectable } from "@angular/core";
import { CanDeactivate } from "@angular/router/src/utils/preactivation";

@Injectable()
export class ManagementReportOnLeave implements CanDeactivate  {

    component: Object;
    route: import("@angular/router").ActivatedRouteSnapshot;

    canDeactivate(): boolean {
        setTimeout(() => {
            window.location.reload();
        }, 100);

        return true;
    }
}