import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MathComponent }  from './math-input.component';
import { FormsModule } from '@angular/forms';
import { AmountFormatModule } from 'app/pipes/amount-format.module';

@NgModule({
    declarations: [ 
        MathComponent 
    ],
    imports: [ 
        CommonModule,
        FormsModule,
        AmountFormatModule
    ],
    providers: [

    ],
    exports: [ 
        MathComponent
     ]
})
export class MathModule {}