import { Component, OnDestroy, ViewChild, } from '@angular/core';
import { Subscription } from "rxjs";
import { MessageService } from '../../../services/common/message.service';
import { WorktimeControlService } from '../../../services/worktime-management/worktime-control.service';
import { I18nService } from '../../../services/common/i18n.service';
import { DataTableService } from '../../../services/common/datatable.service';
import { MenuService } from '../../../services/admin/menu.service';
import { UtilsService } from 'app/services/common/utils.service';
declare var $: any;
declare var moment: any;
import * as FileSaver from "file-saver";

@Component({
  selector: 'app-worktime-control',
  templateUrl: './worktime-control.component.html',
  styleUrls: ['./worktime-control.component.scss']
})
export class WorkTimeControlComponent implements OnDestroy  {

  subscription: Subscription;
  private idKey = 'id';
  private textKey = 'text';
  private nullId = '';
  public analytics: any[] = new Array();
  public analyticId: string = null;
  public model: any;
  public data: any[] = new Array();
  public gridSelector = '#gridTable';
  public loading = false;
  public closeMonths: any[];
  public closeMonthId: any;

  constructor(private worktimeControlService: WorktimeControlService,
    private utilsService: UtilsService,
    private messageService: MessageService,
    public menuService: MenuService,
    private datatableService: DataTableService,
    private i18nService: I18nService) {
  }

    ngOnInit() {
      this.getAnalytics();
    }

    getAnalytics() {
        this.loading = true;
        this.messageService.showLoading();
        this.subscription = this.worktimeControlService.getAnalyticOptionsByCurrentManager().subscribe(res => {
            this.messageService.closeLoading();
            this.analytics = this.sortAnalytics(res.data);
            this.setAnalyticSelect();
            this.getCloseMonths();
        },
        err => {
            this.loading = false;
            this.messageService.closeLoading();
        });
    }

    getCloseMonths(){
        this.subscription = this.utilsService.getCloseMonths().subscribe(data => {
            this.closeMonths = data;
            this.setDefaultCloseMonth();
        });
    }

    setDefaultCloseMonth(){
        if(this.closeMonths && this.closeMonths.length > 0){
            let data = this.closeMonths[0];
            this.closeMonthId = data.id;
        }

        this.getData();
    }

    setAnalyticSelect() {
      const data = this.mapToSelect(this.analytics);
      const self = this;

      $('#analyticControl').select2({
          data: data,
          allowClear: true,
          placeholder: this.i18nService.translateByKey('selectAnOption')
      });
      $('#analyticControl').on('select2:unselecting', function(){
          self.analyticId = null;
          self.getData();
      });
      $('#analyticControl').on('select2:select', function(evt){
          const item = evt.params.data;
          self.analyticId = item.id === this.nullId ? null : item.id;
          self.getData();
      });
    }

    sortAnalytics(data: Array<any>) {
      return data.sort(function (a, b) {
          return a.text.localeCompare(b.text);
        });
    }

    mapToSelect(data: Array<any>): Array<any> {
        const result = new Array<any>();
        result.push({id: this.nullId, text: ''});
        data.forEach(s => {
            const text = s[this.textKey];
            result.push({
                id: s[this.idKey],
                text: text,
                selected: false
            });
        });
        return result;
    }

    ngOnDestroy(): void {
        if(this.subscription) this.subscription.unsubscribe();
    }

    getData() {
        const model = {
            analyticId : this.analyticId,
            closeMonthId: this.closeMonthId
        };
        this.loading = true;
        this.messageService.showLoading();
        this.subscription = this.worktimeControlService.getWorkTimeApproved(model).subscribe(res => {
            this.loading = false;
            this.messageService.closeLoading();
            this.model = res.data;
            this.data = res.data.resources;
            this.initGrid();
        },
        err => {
            this.loading = false;
            this.messageService.closeLoading();
        });
    }

    initGrid(){
        const columns = [{
            "className": 'details-control',
            "orderable": false,
            "data": null,
            "defaultContent": ''
        }, 2, 3, 4, 5, 6, 7, 8, 9, 10];

        const params = {
            selector: this.gridSelector,
            columns: columns,
            columnDefs: [
                { "targets": [ 1 ], "visible": false, "searchable": false }
            ]
        }

        this.datatableService.destroy(this.gridSelector);

        this.datatableService.initialize(params);

        this.updateTableDetail();
    }

    formatDetail(data) {
        const id = $(data[0]).data("id");
        const item = <any>this.data.find(x => x.id == id);

        const details = item != null ? item.details : [];

        let tbody = "";

        details.forEach(x => {
            tbody += this.getRowDetailForma(x);
        });

        return '<div style="margin-left:20px;margin-right:100px"><table class="table table-striped">' +
            '<thead>' +
                '<th>' + this.i18nService.translateByKey('workTimeManagement.date') + '</th>' +
                '<th>' + this.i18nService.translateByKey('workTimeManagement.reference') + '</th>' +
                '<th class="column-xxlg">' + this.i18nService.translateByKey('workTimeManagement.comments') + '</th>' +
                '<th>' + this.i18nService.translateByKey('ADMIN.task.task') + '</th>' +
                '<th>' + this.i18nService.translateByKey('ADMIN.task.category') + '</th>' +
                '<th>' + this.i18nService.translateByKey('workTimeManagement.hours') + '</th>' +
            '</thead>' +
            '<tbody>' + tbody + '</tbody>' +
        '</table></div>';
    }

    updateTableDetail() {
        const self = this;

        $(document).ready(function() {
            const dataTableSelector = self.gridSelector + ' tbody';

            $(dataTableSelector).unbind('click');
            $(dataTableSelector).on('click', 'td.details-control', function () {
                const datatable = $(self.gridSelector).DataTable();
                const tr = $(this).closest('tr');
                const tdi = tr.find("i.fa");
                const row = datatable.row(tr);

                if (row.child.isShown()) {
                    row.child.hide();
                    tr.removeClass('shown');
                    tdi.first().removeClass('fa-minus-square');
                    tdi.first().addClass('fa-plus-square');
                } else {
                    row.child(self.formatDetail(row.data())).show();
                    tr.addClass('shown');
                    tdi.first().removeClass('fa-plus-square');
                    tdi.first().addClass('fa-minus-square');
                }
            });
        });
    }

    getRowDetailForma(item) {
        return '<tr>' +
            '<td>' + moment(item.date).format("DD/MM/YYYY") + '</td>' +
            '<td>' + (item.reference != null ? item.reference :'')  + '</td>' +
            '<td class="column-xxlg text-ellipsis">' + (item.comments != null ? item.comments :'')  + '</td>' +
            '<td>' + item.taskDescription + '</td>' +
            '<td>' + item.categoryDescription + '</td>' +
            '<td>' + item.registeredHours + '</td>' +
            '</tr>';
    }

    export(){
        this.messageService.showLoading();

        this.worktimeControlService.createReport().subscribe(file => {
            this.messageService.closeLoading();

            FileSaver.saveAs(file, "reporte-control-horas.xlsx");
        },
        () => {
            this.messageService.closeLoading();
        });
    }
}
