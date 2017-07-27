import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'icheck',
  templateUrl: './icheck.component.html',
  styleUrls: ['./icheck.component.scss']
})
export class ICheckComponent implements OnInit {

  @Input() checked: boolean = false;

  @Output() click = new EventEmitter<boolean>();

  constructor() { }

  ngOnInit() {
  }

  onClick(){
    this.checked = !this.checked;
    this.click.emit(this.checked);
  }

}
