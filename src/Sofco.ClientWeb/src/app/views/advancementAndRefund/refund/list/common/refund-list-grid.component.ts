import { Component, OnInit, Input } from "@angular/core";
import { DataTableService } from "app/services/common/datatable.service";
import { I18nService } from "app/services/common/i18n.service";

@Component({
    selector: 'refund-list-grid',
    templateUrl: './refund-list-grid.component.html',
    styleUrls: ['./refund-list-grid.component.scss']
})
export class RefundListGridComponent implements OnInit {
    @Input()
    public controlId = 'grid1';
    public loading = false;

    constructor(private datatableService: DataTableService,
        private i18nService: I18nService){}

    ngOnInit(): void {
        this.initGrid();
    }

    initGrid(){
        const gridSelector = "#gridTable-" + this.controlId;

        const columns = [1, 2, 3, 4, 5, 6, 7];

        const params = {
            selector: gridSelector,
            columns: columns,
            title: this.i18nService.translateByKey('refund.listTitle'),
            withExport: true,
            columnDefs: []
        }

        this.datatableService.destroy(gridSelector);

        this.datatableService.initialize(params);
    }
}
