import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'icheck',
  templateUrl: './icheck.component.html',
  styleUrls: ['./icheck.component.scss']
})
export class ICheckComponent {

  @Input() checked: boolean = false;
  @Input() checkAtLeft: boolean = true;
  @Input() disabled: boolean = false;

  @Output() checkedChange = new EventEmitter<boolean>();

  constructor() { }

  onClick(){
    if(!this.disabled){
      this.checked = !this.checked;
      this.checkedChange.emit(this.checked);
    }
  }
}
