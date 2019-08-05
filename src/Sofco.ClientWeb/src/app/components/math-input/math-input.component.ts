import { Component, Input, Output, EventEmitter } from '@angular/core';
import { evaluate, compile, parse } from 'mathjs/number'

@Component({
  selector: 'math-input',
  templateUrl: './math-input.component.html'
})
export class MathComponent {

  @Input() model: string;
//   @Input() disabled = false;
//   @Input() label = "";

//   @Output() checkedChange = new EventEmitter<boolean>();

  constructor() { }

  onEnter(mathBox, value: string) {
    try {
        return evaluate(value)
    } catch (error) {
        mathBox.value = 0
        return 0
    }
}
}