import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { Cookie } from "ng2-cookies/ng2-cookies";
import { CustomerService } from 'app/services/billing/customer.service';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { debug } from 'util';
import { I18nService } from 'app/services/common/i18n.service';
declare var $:any;

@Component({
    selector: 'solfac-account-control',
    templateUrl: './solfac-account-control.component.html',
    styleUrls: ['./solfac-account-control.component.scss']
  })

export class SolfacAccountControlComponent implements OnInit {

    @Input()
    public model:any;

    @Input()
    public integratorProject: any;

    @Input()
    public callbackAccountDetail = false;

    @Output()
    public modelChange: EventEmitter<any> = new EventEmitter<any>();

    private idKey = "id";
    private textKey = "text";

    private customers: any[] = new Array<any>();
    public selected: any;

    constructor(private customerService: CustomerService,
        private errorHandlerService: ErrorHandlerService,
        private i18nService: I18nService) {
    }

    ngOnInit() {
        this.getCustomers();
    }

    getCustomers(){
        this.customerService.getOptions().subscribe(response => {
            this.customers = this.sortCustomers(response.data);
            this.setSelect();
        },
        err => {
            this.errorHandlerService.handleErrors(err)
        });
    }

    sortCustomers(data:Array<any>) {
        return data.sort(function (a, b) {
            return a.text.localeCompare(b.text);
          });
    }

    setSelect() {
        let data = this.getData();
        var self = this;

        $("#accountControl").select2({data: data});
        $("#accountControl").on('select2:select', function(evt){
            var data = evt.params.data;
            self.model = data.text;
            //self.modelChange.emit(self.model);
            self.getSelectedDetail(data.id);
        });
    }

    getSelectedDetail(id:string) {
        if(!this.callbackAccountDetail) {
            this.modelChange.emit(this.model);
            return;
        };

        this.customerService.getById(id).subscribe(data => {
            this.selected = data;
            this.modelChange.emit(this.model);
        },
        err => {
            this.errorHandlerService.handleErrors(err)
        });
    }

    getData():Array<any> {
        let accounts = this.mapToSelect(this.customers, this.model);

        if(!this.hasIntegrator()) return accounts;

        let data = [
            {
                "text": this.i18nService.translateByKey("billing.solfac.integrator"), 
                "children" : [
                    {
                        "id": this.integratorProject.integratorId,
                        "text": this.integratorProject.integrator
                    }
                ]
            },
            {
                "text": this.i18nService.translateByKey("billing.solfac.customers"),
                "children" : accounts
            }
          ];

        return data;
    }

    mapToSelect(data:Array<any>, selectedOption:string):Array<any> {
        let result = new Array<any>();
    
        data.forEach(s => {
            let text = s[this.textKey];
            if(this.integratorProject == null 
                || this.integratorProject.integrator != text) {
                let selected:boolean = text == selectedOption;
                result.push({
                    id: s[this.idKey],
                    text: text,
                    selected: selected
                });
            }
        })
    
        return result;
      }
    
      hasIntegrator():boolean {
          let result = false;
          if(this.integratorProject != null 
            && this.integratorProject.integrator != "")
          {
            result = true;
          }
          return result;
      }
}