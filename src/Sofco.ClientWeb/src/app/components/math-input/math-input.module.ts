import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MathComponent }  from './math-input.component';
import { FormsModule } from '@angular/forms';

@NgModule({
    declarations: [ 
        MathComponent 
    ],
    imports: [ 
        CommonModule,
        FormsModule
    ],
    providers: [

    ],
    exports: [ 
        MathComponent
     ]
})
export class MathModule {}