import { Component, Input, OnInit  } from '@angular/core';

@Component({
    selector: 'app-worktime-approver',
    templateUrl: './worktime-approver.component.html'
  })

export class WorkTimeApproverComponent implements OnInit {

    @Input()
    public model: any;

    public loading = false;

    constructor() {
    }

    ngOnInit() {
    }

    ngOnDestroy() {
    }
}
