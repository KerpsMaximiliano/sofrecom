import { Component, OnInit, OnDestroy} from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { Solfac } from 'models/billing/solfac/solfac';
import { HitoDetail } from "models/billing/solfac/hitoDetail";
import { SolfacService } from "app/services/billing/solfac.service";
import { Option } from "models/option";
import { Router, ActivatedRoute } from '@angular/router';
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Cookie } from "ng2-cookies/ng2-cookies";
import { MessageService } from "app/services/common/message.service";
import { UserService } from "app/services/admin/user.service";

@Component({
  selector: 'app-solfac',
  templateUrl: './solfac.component.html',
  styleUrls: ['./solfac.component.scss']
})
export class SolfacComponent implements OnInit, OnDestroy {

    public model: Solfac = <Solfac>{};
    public provinces: Option[] = new Array<Option>();
    public documentTypes: Option[] = new Array<Option>();
    public imputationNumbers: Option[] = new Array<Option>();
    public currencies: Option[] = new Array<Option>();
    public users: any[] = new Array();
    public currencySymbol: string = "$";
    private projectId: string = "";

    getOptionsSubs: Subscription;

    constructor(private messageService: MessageService,
                private solfacService: SolfacService,
                private userService: UserService,
                private errorHandlerService: ErrorHandlerService,
                private router: Router) { }

    ngOnInit() {
      this.getOptions();
      this.getUserOptions();

      var project = JSON.parse(sessionStorage.getItem('projectDetail'));
      var customer = JSON.parse(sessionStorage.getItem("customer"));

      this.projectId = project.id;

      this.model.businessName = customer.nombre;
      this.model.clientName = customer.contact;
      this.model.celphone = customer.telephone;
      this.model.statusName = "Pendiente de envío";
      this.model.contractNumber = project.purchaseOrder;
      this.model.project = project.nombre;
      this.model.projectId = project.id;
      this.model.documentTypeId = 1;
      this.model.amount = 0;
      this.model.iva21 = 0;
      this.model.totalAmount = 0;
      this.model.imputationNumber3 = 1;
      this.model.currencyId = 1;

      this.model.hitos = new Array<HitoDetail>();
      var hitos = JSON.parse(sessionStorage.getItem('hitosSelected'));

      hitos.forEach(hito => {
        var hitoDetail = new HitoDetail(hito.name, 1, hito.ammount, project.id, hito.id);
        this.model.hitos.push(hitoDetail);
      });

      this.calculate();
    }

    ngOnDestroy(){
       if(this.getOptionsSubs) this.getOptionsSubs.unsubscribe();
    }

    getUserOptions(){
        this.userService.getOptions().subscribe(data => {
          this.users = data;

          var userapplicant = this.users.find(x => x.userName == Cookie.get('currentUser'));
          this.model.userApplicantId = userapplicant.value;
          this.model.userApplicantName = userapplicant.text;
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    getOptions(){
      this.getOptionsSubs = this.solfacService.getOptions().subscribe(data => {
        this.currencies = data.currencies;
        this.provinces = data.provinces;
        this.documentTypes = data.documentTypes;
        this.imputationNumbers = data.imputationNumbers;
      },
      err => this.errorHandlerService.handleErrors(err));
    }

    calculate(){
      this.model.hitos.forEach(item => {
        this.calculateDetail(item);
      });

      this.calculateAmounts();
    }

    calculateDetail(detail: HitoDetail){
      if(detail.quantity > 0 && detail.unitPrice > 0){

        if(this.model.documentTypeId == 3 || this.model.documentTypeId == 4){
            detail.total = detail.quantity * (detail.unitPrice * 1.21);
        }
        else{
          detail.total = detail.quantity * detail.unitPrice;
        }
      }
      else{
        detail.total = 0;
      }
    }

    calculateTotal(detail: HitoDetail){
      this.calculateDetail(detail);
      this.calculateAmounts();
    }

    changeDocumentType(){
      this.model.hitos.forEach(detail => {
        this.calculateTotal(detail);
      });
    }

    calculateAmounts(){
      this.model.amount = 0;

      this.model.hitos.forEach(detail => {
        this.model.amount += detail.total;
      });

      if(this.model.documentTypeId == 1 || this.model.documentTypeId == 2){
        this.model.iva21 = this.model.amount * 0.21;
      }
      else{
        this.model.iva21 = 0;
      }

      this.model.totalAmount = this.model.amount + this.model.iva21;
    }

    save(){
      this.solfacService.add(this.model).subscribe(
        data => {
          if(data.messages) this.messageService.showMessages(data.messages);

          setTimeout(() => {
            this.router.navigate([`/billing/project/${this.projectId}`]);
          }, 1500)
        },
        err => this.errorHandlerService.handleErrors(err));
    }

    setCurrencySymbol(currencyId){
      switch(currencyId){
        case "1": { this.currencySymbol = "$"; break; }
        case "2": { this.currencySymbol = "U$D"; break; }
        case "3": { this.currencySymbol = "€"; break; }
      }
    }

    cancel(){
      this.router.navigate([`/billing/project/${this.projectId}`]);
    }
}