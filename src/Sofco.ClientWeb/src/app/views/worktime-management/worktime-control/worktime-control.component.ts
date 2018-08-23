import { Component, OnDestroy, ViewChild, } from '@angular/core';
import { Subscription } from "rxjs";
import { MessageService } from '../../../services/common/message.service';
import { WorktimeControlService } from '../../../services/worktime-management/worktime-control.service';
import { DateRangePickerComponent } from '../../../components/date-range-picker/date-range-picker.component';
import { CustomerService } from '../../../services/billing/customer.service';
import { ServiceService } from '../../../services/billing/service.service';
import { I18nService } from '../../../services/common/i18n.service';
import { DataTableService } from '../../../services/common/datatable.service';
import { MenuService } from '../../../services/admin/menu.service';
import { AmountFormatPipe } from '../../../pipes/amount-format.pipe';
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
  public customers: any[] = new Array();
  public customerId: string = null;
  private services: any[] = new Array<any>();
  public serviceId: string = null;
  public model: any;
  public date = new Date();
  public selectedDate = new Date();
  public data: any[] = new Array();
  public gridSelector = '#gridTable';

  @ViewChild('dateRangePicker') dateRangePicker:DateRangePickerComponent;

  constructor(private serviceService: ServiceService,
    private customerService: CustomerService,
    private worktimeControlService: WorktimeControlService,
    private messageService: MessageService,
    public menuService: MenuService,
    private datatableService: DataTableService,
    private i18nService: I18nService) {
  }

    ngOnInit() {
      this.getCustomers();
      this.initServiceControl();
      this.initControls();
      this.getData();
    }

    getCustomers() {
      this.messageService.showLoading();
      this.subscription = this.customerService.getOptions().subscribe(res => {
          this.messageService.closeLoading();
          this.customers = this.sortCustomers(res.data);
          this.setCustomerSelect();
      },
      err => {
          this.messageService.closeLoading();
      });
    }

    setCustomerSelect() {
      const data = this.mapToSelect(this.customers);
      const self = this;

      $('#customerControl').select2({
          data: data,
          allowClear: true,
          placeholder: this.i18nService.translateByKey('selectAnOption')
      });
      $('#customerControl').on('select2:unselecting', function(){
          self.customerId = null;
          self.serviceId = null;
          self.getData();
      });
      $('#customerControl').on('select2:select', function(evt){
          const item = evt.params.data;
          self.customerId = item.id === this.nullId ? null : item.id;
          self.getServices();
      });
  }

  initServiceControl() {
    const self = this;
    $('#serviceControl').select2({
        data: [],
        allowClear: true,
        placeholder: this.i18nService.translateByKey('selectAnOption')
    });
    $('#serviceControl').on('select2:unselecting', function(){
        self.serviceId = null;
        self.getData();
    });
    $('#serviceControl').on('select2:select', function(evt){
        const item = evt.params.data;
        self.serviceId = item.id === this.nullId ? null : item.id;
        self.getData();
    });
  }

  sortCustomers(data: Array<any>) {
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

  getServices(): void {
    if (this.customerId === null) { return; }
    this.subscription = this.serviceService.getOptions(this.customerId).subscribe(res => {
        this.services = res.data;
        this.updateServiceControl();
    });
  }

  updateServiceControl() {
      const options = $('#serviceControl').data('select2').options.options;
      options.data = this.mapToSelect(this.services);
      $('#serviceControl').empty().select2(options);
  }

  searchCriteriaChange() {}

  ngOnDestroy(): void {
    if(this.subscription) this.subscription.unsubscribe();
  }

  showService(): boolean {
    return this.customerId !== null;
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
        this.messageService.showLoading();
        const model = {
            serviceId : this.serviceId,
            year: this.date.getFullYear(),
            month: this.date.getMonth() + 1
        };
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
        }, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

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
                '<th>' + this.i18nService.translateByKey('ADMIN.task.task') + '</th>' +
                '<th>' + this.i18nService.translateByKey('ADMIN.task.category') + '</th>' +
                '<th>' + this.i18nService.translateByKey('workTimeManagement.date') + '</th>' +
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
                '<td>' + item.taskDescription + '</td>' +
                '<td>' + item.categoryDescription + '</td>' +
                '<td>' + moment(item.date).format("DD/MM/YYYY") + '</td>' +
                '<td>' + item.registeredHours + '</td>' +
                '</tr>';
    }
}
