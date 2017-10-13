import { Router} from '@angular/router';
import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { ErrorHandlerService } from "app/services/common/errorHandler.service";
import { DataTableService } from "app/services/common/datatable.service";
import { MenuService } from 'app/services/admin/menu.service';

@Component({
    selector: 'app-solfac-report',
    templateUrl: './solfac.component.html'
})

export class SolfacReportComponent implements OnInit, OnDestroy {
    public loading: boolean = true;
    public dateOptions;
    dateSince: Date = new Date();
    dateTo: Date = new Date();

    constructor (
        private router: Router,
        private menuService: MenuService,
        private datatableService: DataTableService,
        private errorHandlerService: ErrorHandlerService) 
    {
        this.dateOptions = this.menuService.getDatePickerOptions();
    }

    ngOnInit() {
        this.init();
    }

    init(){
        var date = new Date();
        this.dateSince = new Date(date.getFullYear(), date.getMonth(), 1);
        this.dateTo = new Date(date.getFullYear(), date.getMonth() + 1, 0);
    }

    ngOnDestroy(){
    }

    getAll(){
    }
}
