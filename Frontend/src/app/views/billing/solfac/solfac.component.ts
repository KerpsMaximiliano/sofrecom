import { MessageService } from 'app/services/message.service';
import { Component, OnInit, OnDestroy} from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { Solfac } from 'models/billing/solfac/solfac';
import { HitoDetail } from "models/billing/solfac/hitoDetail";
import { SolfacService } from "app/services/billing/solfac.service";
import { Option } from "models/option";
import { UserService } from "app/services/user.service";

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
    public users: Option[] = new Array<Option>();

    getOptionsSubs: Subscription;

    constructor(private messageService: MessageService,
                private solfacService: SolfacService,
                private userService: UserService) { }

    ngOnInit() {
      this.getOptions();
      this.getUserOptions();
      this.model.businessName = "Sofrecom Argentina S.A.";
      this.model.clientName = "Juan Jose Larenze";
      this.model.celphone = "1123458373";
      this.model.statusName = "Pendiente de envío";
      this.model.contractNumber = "11660039";
      this.model.project = "SofcoAr";
      this.model.documentTypeId = 1;
      this.model.userApplicantId = 1;
      this.model.hitos = new Array<HitoDetail>();
      this.model.amount = 0;
      this.model.iva21 = 0;
      this.model.totalAmount = 0;
      this.model.imputationNumber3 = 1;
      this.model.currencyId = 1;

      var hitoDetail1 = new HitoDetail(1, "Hito 1", 0, 0, 0);
      var hitoDetail2 = new HitoDetail(2, "Hito 2", 0, 0, 0);
      var hitoDetail3 = new HitoDetail(3, "Hito 3", 0, 0, 0);

      this.model.hitos.push(hitoDetail1);
      this.model.hitos.push(hitoDetail2);
      this.model.hitos.push(hitoDetail3);
    }

    ngOnDestroy(){
       if(this.getOptionsSubs) this.getOptionsSubs.unsubscribe();
    }

    getUserOptions(){
        this.userService.getOptions().subscribe(data => {
          this.users = data;
        },
        err => {
          console.log(err);
        });
    }

    getOptions(){
      this.getOptionsSubs = this.solfacService.getOptions().subscribe(data => {
        this.currencies = data.currencies;
        this.provinces = data.provinces;
        this.documentTypes = data.documentTypes;
        this.imputationNumbers = data.imputationNumbers;
      },
      err => {
        console.log(err);
      });
    }

    calculateTotal(detail: HitoDetail){
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
          console.log(data);
          if(data.messages) this.messageService.showMessages(data.messages);
        },
        err => {
          var json = JSON.parse(err._body)
          if(json.messages) this.messageService.showMessages(json.messages);
        }
      );
    }
}