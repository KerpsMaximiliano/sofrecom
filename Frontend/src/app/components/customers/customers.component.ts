import { Ng2ModalConfig } from 'app/shared/modal/ng2modal-config';

import { Customer } from 'models/customer';
import { CustomersService } from 'app/services/customers.service';
import { Component, OnInit } from '@angular/core';
import { FormBuilder } from "@angular/forms";

@Component({
  selector: 'app-customers',
  templateUrl: './customers.component.html',
  styleUrls: ['./customers.component.scss']
})
export class CustomersComponent implements OnInit {

  public searchTerm: string;
  public _customers: Customer[] = [];
  private _customersCopy: Customer[] = [];
  private modalConfig: Ng2ModalConfig = new Ng2ModalConfig('Modal 1 Title', 'modal1', true, true, "Ok", "Cancel");
  

  constructor(private service: CustomersService, private _fb: FormBuilder) { }

  ngOnInit() {
    this.loadData();
  }

  search():void {
        let term = this.searchTerm;
        let listfilters = this._customersCopy.filter(item => item.Name.toLowerCase().indexOf(term.toLowerCase()) > -1);
        this._customers = listfilters;
    }

  private loadData():void {
      this.service.getAll().subscribe(data => { 
          this._customers = data;
          this._customersCopy = data; 
      });
  }

  modal1Accept(){
    
  }

}
