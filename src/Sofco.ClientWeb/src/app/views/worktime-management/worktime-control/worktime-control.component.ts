import { Component, OnDestroy, ViewChild, } from '@angular/core';
import { Subscription } from "rxjs";
import { MessageService } from '../../../services/common/message.service';
import { WorktimeControlService } from '../../../services/worktime-management/worktime-control.service';
import { DateRangePickerComponent } from '../../../components/date-range-picker/date-range-picker.component';
import { I18nService } from '../../../services/common/i18n.service';
import { DataTableService } from '../../../services/common/datatable.service';
import { MenuService } from '../../../services/admin/menu.service';
import { AnalyticService } from 'app/services/allocation-management/analytic.service';
declare var $: any;
declare var moment: any;

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
  public date = new Date();
  public selectedDate = new Date();
  public data: any[] = new Array();
  public gridSelector = '#gridTable';
  public startDate: Date;
  public endDate: Date;

  @ViewChild('dateRangePicker') dateRangePicker:DateRangePickerComponent;

  constructor(private analyticService: AnalyticService,
    private worktimeControlService: WorktimeControlService,
    private messageService: MessageService,
    public menuService: MenuService,
    private datatableService: DataTableService,
    private i18nService: I18nService) {
  }

    ngOnInit() {
      this.getAnalytics();
      this.initControls();
    }

    getAnalytics() {
      this.messageService.showLoading();
      this.subscription = this.analyticService.getByManager().subscribe(res => {
        this.messageService.closeLoading();
        this.analytics = this.sortAnalytics(res.data);
        this.setAnalyticSelect();
      },
      err => {
          this.messageService.closeLoading();
      });
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

  initControls() {
    const self = this;

    $('#yearMonthControl').datepicker({
        autoclose: true,
        minViewMode: 1,
        format: 'yyyy - mm'
    }).on('changeDate', function(selected){
        self.date = selected.date;
        self.getData();
    });
    $('#yearMonthControl').datepicker('update', self.date);
  }

    getData() {
        if(this.dateRangePicker) {
            this.startDate = this.dateRangePicker.start.toDate();
            this.endDate = this.dateRangePicker.end.toDate();
            this.endDate.setHours(0,0,0,0);
        }

        const model = {
            analyticId : this.analyticId,
            startDate: this.startDate,
            endDate: this.endDate
        };
        this.messageService.showLoading();
        this.subscription = this.worktimeControlService.getWorkTimeApproved(model).subscribe(res => {
            this.messageService.closeLoading();
            this.model = res.data;
            this.data = res.data.resources;
            this.initGrid();
        },
        err => {
            this.messageService.closeLoading();
        });
    }

    initGrid(){
        const columns = [{
            "className": 'details-control',
            "orderable": false,
            "data": null,
            "defaultContent": ''
        }, 2, 3, 4, 5, 6, 7, 8, 9];

        const params = {
            selector: this.gridSelector,
            columns: columns,
            title: this.i18nService.translateByKey('workTimeManagement.worktimeControl.title'),
            withExport: true,
            columnDefs: [
                { "targets": [ 1 ], "visible": false, "searchable": false }
            ]
        }

        this.datatableService.destroy(this.gridSelector);

        this.datatableService.initialize(params);

        this.updateTableDetail();
    }

    formatDetail( data ) {
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
                '<th>' + this.i18nService.translateByKey('ADMIN.task.task') + '</th>' +
                '<th>' + this.i18nService.translateByKey('ADMIN.task.category') + '</th>' +
                '<th>' + this.i18nService.translateByKey('workTimeManagement.worktimeControl.registeredHours') + '</th>' +
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
            '<td>' + item.taskDescription + '</td>' +
            '<td>' + item.categoryDescription + '</td>' +
            '<td>' + item.registeredHours + '</td>' +
            '</tr>';
    }
}
