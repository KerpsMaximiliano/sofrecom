import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from "rxjs";
import { ActivatedRoute } from '@angular/router';
import { FileService } from '../../../services/common/file.service';
import { MessageService } from '../../../services/common/message.service';
declare var PDFJS: any;

@Component({
  selector: 'pdf',
  templateUrl: './pdf.component.html'
})
export class PdfComponent implements OnInit, OnDestroy  {

    subscrip: Subscription;
    paramsSubscrip: Subscription;

    canvas: any;
    ctx: any;

    pdfDoc: any;

    public documentPagesNumber: number;
    public pageNum: number = 1;
    scale: number = 1;
    pageRendering: boolean;
    pageNumPending: number;
    zoomRange: number = 0.25

    private id: number;
    private type: number;

    constructor(private activatedRoute: ActivatedRoute, 
        private fileService: FileService,
        private messageService: MessageService) { }

    ngOnInit(): void {
        this.paramsSubscrip = this.activatedRoute.params.subscribe(params => {
            this.id = params['id'];
            this.type = params['type'];

            this.getPdf();
        });

        this.canvas = document.getElementById('the-canvas');
        this.ctx = this.canvas.getContext('2d');

        PDFJS.workerSrc = 'assets/js/pdfjs/pdf.worker.js';
    }

    ngOnDestroy(): void {
        if(this.subscrip) this.subscrip.unsubscribe();
        if(this.paramsSubscrip) this.paramsSubscrip.unsubscribe();
    }

    getPdf(){
        this.messageService.showLoading();

        this.fileService.getFile(this.id, this.type).subscribe(response => {
            this.messageService.closeLoading();

            this.renderFile(response.data);
        },
        () => this.messageService.closeLoading());
    }

    renderFile(file){
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