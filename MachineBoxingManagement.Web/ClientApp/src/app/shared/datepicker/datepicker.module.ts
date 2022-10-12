import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { NgbdDatepickerPopup } from './datepicker.component';

@NgModule({
  imports: [BrowserModule, NgbModule, FormsModule],
  declarations: [NgbdDatepickerPopup],
  exports: [NgbdDatepickerPopup],
  bootstrap: [NgbdDatepickerPopup]
})
export class NgbdDatepickerPopupModule { }
