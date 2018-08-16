import { Component, OnDestroy, ViewChild, Output, EventEmitter } from '@angular/core';
import { Ng2ModalConfig } from '../../../components/modal/ng2modal-config';
import { Subscription } from "rxjs";
import { MessageService } from '../../../services/common/message.service';
import { WorktimeService } from '../../../services/worktime-management/worktime.service';
import { DateRangePickerComponent } from '../../../components/date-range-picker/date-range-picker.component';
import { CustomerService } from '../../../services/billing/customer.service';
import { ServiceService } from '../../../services/billing/service.service';
import { UserService } from '../../../services/admin/user.service';
import { I18nService } from '../../../services/common/i18n.service';
import { Router } from '../../../../../node_modules/@angular/router';
declare var $: any;

@Component({
  selector: 'app-worktime-control',
  templateUrl: './worktime-control.component.html'
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

  @ViewChild('dateRangePicker') dateRangePicker:DateRangePickerComponent;

  constructor(private serviceService: ServiceService,
    private customerService: CustomerService,
    private usersService: UserService,
    private messageService: MessageService,
    private i18nService: I18nService,
    private router: Router) {
  }

    ngOnInit() {
      this.getCustomers();
      this.initServiceControl();
      this.initControls();
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
      const data = this.mapToSelect(this.customers, this.model);
      const self = this;

      $('#customerControl').select2({
          data: data,
          allowClear: true,
          placeholder: this.i18nService.translateByKey('selectAnOption')
      });
      $('#customerControl').on('select2:unselecting', function(){
          self.customerId = null;
          self.serviceId = null;
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
    });
    $('#serviceControl').on('select2:select', function(evt){
        const item = evt.params.data;
        self.serviceId = item.id === this.nullId ? null : item.id;
    });
  }

  sortCustomers(data: Array<any>) {
      return data.sort(function (a, b) {
          return a.text.localeCompare(b.text);
        });
  }

  mapToSelect(data: Array<any>, selectedOption: string): Array<any> {
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
      options.data = this.mapToSelect(this.services, this.model);
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
        format: 'yyyy - mm',
        language: 'es'
    }).on('changeDate', function(selected){
        self.date = selected.date;
    });
    $('#yearMonthControl').datepicker('update', self.date);
  }
}
