import { Component, OnInit, OnDestroy } from "@angular/core";
import { FormsService } from "app/services/forms/forms.service";
import { Setting } from "app/models/advancement-and-refund/setting";
import { Subscription } from "rxjs";
import { AdvancementRefundSettingService } from "app/services/advancement-and-refund/setting.service";
import { MessageService } from "app/services/common/message.service";
import { I18nService } from "app/services/common/i18n.service";

@Component({
    selector: 'advancement-refund-setting',
    templateUrl: './setting.component.html'
})
export class AdvancementRefundSettingComponent implements OnInit, OnDestroy  {
    public form: Setting;

    getSubscrip: Subscription;
    postSubscrip: Subscription;

    constructor(public formsService: FormsService, 
                private messageService: MessageService,
                private settingService: AdvancementRefundSettingService,
                public i18nService: I18nService){}

    ngOnInit(): void {
        this.messageService.showLoading();

        this.getSubscrip = this.settingService.get().subscribe(response => {
            this.messageService.closeLoading();
            this.form = new Setting(response.data);
        },
        error => this.messageService.closeLoading());
    }

    ngOnDestroy(): void {
        if(this.getSubscrip) this.getSubscrip.unsubscribe();
        if(this.postSubscrip) this.postSubscrip.unsubscribe();
    }

    canSave(){
        return this.formsService.canSave(this.form);
    }

    save(){
        var model = this.form.getModel();

        this.messageService.showLoading();

        this.postSubscrip = this.settingService.update(model).subscribe(response => {
            this.messageService.closeLoading();
        },
        error => this.messageService.closeLoading());
    }
}