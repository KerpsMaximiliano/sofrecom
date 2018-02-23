import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Cookie } from 'ng2-cookies/ng2-cookies';
import { ServiceService } from 'app/services/billing/service.service';
import { CustomerService } from 'app/services/billing/customer.service';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { I18nService } from 'app/services/common/i18n.service';
import { UserService } from 'app/services/admin/user.service';
declare var $:any;

@Component({
    selector: 'app-solfac-delegate',
    templateUrl: './solfac-delegate.component.html'
  })

export class SolfacDelegateComponent implements OnInit {

    private nullId = '';

    @Input()
    public model: any;

    private idKey = 'value';
    private textKey = 'text';

    private customers: any[] = new Array<any>();
    public customerId: string = null;

    private services: any[] = new Array<any>();
    public serviceId: string = null;

    public users: any[] = new Array<any>();
    public userId: string = null;

    constructor(private serviceService: ServiceService,
        private customerService: CustomerService,
        private usersService: UserService,
        private errorHandlerService: ErrorHandlerService,
        private i18nService: I18nService) {

    }

    ngOnInit() {
        this.getCustomers();
        this.initServiceControl();
        this.getUsers();
    }

    getCustomers(){
        this.customerService.getOptions(Cookie.get('currentUserMail')).subscribe(data => {
            this.customers = this.sortCustomers(data);
            this.setCustomerSelect();
        },
        err => {
            this.errorHandlerService.handleErrors(err);
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
        this.serviceService.getOptions(this.customerId).subscribe(services => {
            this.services = services;
            this.updateServiceControl();
        },
        err => {
            this.errorHandlerService.handleErrors(err);
        });
    }

    updateServiceControl() {
        const options = $('#serviceControl').data('select2').options.options;
        options.data = this.mapToSelect(this.services, this.model);
        $('#serviceControl').empty().select2(options);
    }

    getUsers(): void {
        this.usersService.getOptions().subscribe(users => {
            this.users = users;
            this.initUserControl();
        },
        err => {
            this.errorHandlerService.handleErrors(err);
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

    initUserControl() {
        const data = this.mapToSelect(this.users, this.model);
        const self = this;

        $('#userControl').select2({
            data: data,
            allowClear: true,
            placeholder: this.i18nService.translateByKey('selectAnOption'),
            width: '100%'
        });
        $('#userControl').on('select2:unselecting', function(){
            self.userId = null;
        });
        $('#userControl').on('select2:select', function(evt){
            const item = evt.params.data;
            self.userId = item.id === this.nullId ? null : item.id;
        });
    }

    showService(): boolean {
        return this.customerId !== null;
    }

    showUser(): boolean {
        return this.serviceId !== null;
    }

    showSave(): boolean {
        return this.userId !== null;
    }

    save(): void {
        console.log('>save > userId[' + this.userId + '] - serviceId[' + this.serviceId + ']')
    }
}
