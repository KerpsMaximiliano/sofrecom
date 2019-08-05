import { Component, Input, Output, EventEmitter } from '@angular/core';
import { evaluate, compile, parse } from 'mathjs/number'

@Component({
  selector: 'math-input',
  templateUrl: './math-input.component.html'
})
export class MathComponent {

  @Input() model;

   @Output() modelChange = new EventEmitter<boolean>();

  constructor() { }

  onEnter(mathBox, value: string) {
    var result;
    
    try {
      result = evaluate(value)

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
}