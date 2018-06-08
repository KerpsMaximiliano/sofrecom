
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PdfViewerComponent } from 'app/components/pdf-viewer/pdf-viewer.component';
import { Ng2ModalModule } from 'app/components/modal/ng2modal.module';
import { FormsModule } from '@angular/forms';

@NgModule({
  imports: [
    CommonModule, Ng2ModalModule, FormsModule
  ],
  declarations: [ 
    PdfViewerComponent
  ],
  providers: [
      
  ],
  exports: [
    PdfViewerComponent
  ]
})
export class PdfViewerModule { }
