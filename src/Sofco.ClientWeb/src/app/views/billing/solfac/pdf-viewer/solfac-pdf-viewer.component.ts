import { Component, OnInit, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { SolfacService } from "app/services/billing/solfac.service";
import { ErrorHandlerService } from 'app/services/common/errorHandler.service';
import { Subscription } from "rxjs/Subscription";
import { MessageService } from 'app/services/common/message.service';
import { InvoiceService } from 'app/services/billing/invoice.service';
declare var PDFJS: any;

@Component({
  selector: 'solfac-pdf-viewer',
  templateUrl: './solfac-pdf-viewer.component.html'
})
export class SolfacPdfViewerComponent implements OnInit, OnDestroy  {

    @ViewChild('pdfModal') pdfModal;
    public pdfModalConfig: Ng2ModalConfig = new Ng2ModalConfig(
        "billing.solfac.pdfViewer",
        "pdfModal", 
        true,
        false,
        "ACTIONS.close",
        "ACTIONS.cancel"
    );

    subscrip: Subscription;

    canvas: any;
    ctx: any;

    constructor(private invoiceService: InvoiceService,
        private messageService: MessageService,
        private errorHandlerService: ErrorHandlerService) { 
    }

    ngOnInit(): void {
        this.canvas = document.getElementById('the-canvas');
        this.ctx = this.canvas.getContext('2d');

        PDFJS.workerSrc = 'assets/js/pdfjs/pdf.worker.js';
    }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
    }

    close(){
        this.pdfModal.hide();
    }

    getPdf(invoiceId){
        this.pdfModal.show();

        var self = this;

        self.invoiceService.getPdf(invoiceId).subscribe(file => {
            var pdfData = atob(file);
            
            PDFJS.getDocument({data: pdfData}).then(function (pdfDoc_) {
                var pageNumber = 1;
                pdfDoc_.getPage(pageNumber).then(function(page) {
                    
                    var scale = 1;
                    var viewport = page.getViewport(scale);
                
                    // Prepare canvas using PDF page dimensions
                    self.canvas.height = viewport.height;
                    self.canvas.width = viewport.width;
                
                    // Render PDF page into canvas context
                    var renderContext = {
                        canvasContext: self.ctx,
                        viewport: viewport
                    };

                    page.render(renderContext);
                });
            });

        },
        err => self.errorHandlerService.handleErrors(err));
    } 
x
}