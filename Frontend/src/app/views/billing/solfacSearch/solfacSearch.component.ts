import { MessageService } from 'app/services/message.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs/Subscription";
import { SolfacService } from "app/services/billing/solfac.service";

@Component({
  selector: 'app-solfacSearch',
  templateUrl: './solfacSearch.component.html'
})
export class SolfacSearchComponent implements OnInit, OnDestroy {
    getAllSubscrip: Subscription;
    solfacs: any[];

    constructor(
        private router: Router,
        private activatedRoute: ActivatedRoute,
        private service: SolfacService,
        private messageService: MessageService) { }

    ngOnInit() {
        this.getAllSubscrip = this.service.getAll().subscribe(data => {
            this.solfacs = data;
        },
        err => {
            console.log(err);
        });
    }

    ngOnDestroy(){
      if(this.getAllSubscrip) this.getAllSubscrip.unsubscribe();
    }
}
