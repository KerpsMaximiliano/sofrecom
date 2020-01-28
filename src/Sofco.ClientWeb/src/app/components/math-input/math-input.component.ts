import { Component, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { evaluate, compile, parse } from 'mathjs/number'

@Component({
  selector: 'math-input',
  templateUrl: './math-input.component.html',
  styleUrls: ['./math-input.component.scss']
})
export class MathComponent {

  @Input() model;
  @Input() modelDisabled: boolean = false

  @Output() modelChange = new EventEmitter<boolean>();

  @ViewChild("mathBox") mathBox;

  inputVisible: boolean = false;

  constructor() { }

  onEnter(mathBox, value: string) {
    var result;

    try {
      if(value == null || value == ""){
        result = 0
      }
      else{
        result = evaluate(value)
      }

      this.model = result
      this.modelChange.emit(this.model);
    }
    catch (error) {
      result = 0

      mathBox.value = result
      this.model = result
      this.modelChange.emit(this.model);
    }

    return result
  }
 
  focusOut(){
    this.inputVisible = false;
  }

  focusIn(){
    this.inputVisible = true;

    setTimeout(() => {
      this.mathBox.nativeElement.focus();
    }, 100);
  }
}