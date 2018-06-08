import { Component, OnInit, OnDestroy, ViewChild, Input, EventEmitter, Output } from '@angular/core';
import { Ng2ModalConfig } from 'app/components/modal/ng2modal-config';
import { Subscription } from "rxjs/Subscription";
declare var PDFJS: any;

@Component({
  selector: 'pdf-viewer',
  templateUrl: './pdf-viewer.component.html'
})
export class PdfViewerComponent implements OnInit, OnDestroy  {

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

    pdfDoc: any;

    public documentPagesNumber: number;
    public pageNum: number = 1;
    scale: number = 1;
    pageRendering: boolean;
    pageNumPending: number;
    zoomRange: number = 0.25

    constructor() { }

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

    renderFile(file){
        this.pdfModal.show();
        var pdfData = atob(file);
        this.render(pdfData);
    } 

    render(pdfData){
        var self = this;
        PDFJS.getDocument({data: pdfData}).then(function (pdfDoc_) {
            self.pdfDoc = pdfDoc_;
            self.documentPagesNumber = self.pdfDoc.numPages;

            self.renderPage(self, self.pageNum);
        });
    }

    renderPage(self, num){
        this.pageRendering = true;

        self.pdfDoc.getPage(num).then(function(page) {
                
            var viewport = page.getViewport(self.scale);
        
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
    }

    queueRenderPage(){
        if (this.pageRendering) {
            this.pageNumPending = this.pageNum;
        } else {
            this.renderPage(this, this.pageNum);
        }
    }

    onPrevPage(){
        if (this.pageNum <= 1) {
            return;
        }
        this.pageNum--;
        this.renderPage(this, this.pageNum);
    }

    onNextPage() {
        if (this.pageNum >= this.pdfDoc.numPages) {
            return;
        }
        this.pageNum++;
        this.renderPage(this, this.pageNum);
    }
}