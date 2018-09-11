import { Component, Input, OnInit  } from '@angular/core';

@Component({
    selector: 'app-worktime-approval-delegate',
    templateUrl: './worktime-approval-delegate.component.html'
  })

export class WorkTimeApprovalDelegateComponent implements OnInit {

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
