import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'pcheck',
  templateUrl: './pcheck.component.html'
})
export class PCheckComponent {

  @Input() checked = false;
  @Input() disabled = false;
  @Input() label = "";

  @Output() checkedChange = new EventEmitter<boolean>();

  constructor() { }

  onClick() {
    if (!this.disabled){
      this.checked = !this.checked;
      this.checkedChange.emit(this.checked);
    }
  }
}
