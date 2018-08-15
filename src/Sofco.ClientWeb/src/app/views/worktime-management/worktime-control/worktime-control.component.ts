import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Subscription } from 'rxjs';
import { DataTableService } from 'app/services/common/datatable.service';

@Component({
  selector: 'app-worktime-control',
  templateUrl: './worktime-control.component.html'
})
export class WorktimeControlComponent implements OnInit, OnDestroy {

  public loading = false;
  subscription: Subscription;

  constructor(private datatableService: DataTableService) {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }
}
