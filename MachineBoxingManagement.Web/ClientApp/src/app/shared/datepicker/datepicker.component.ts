import { Component, Input, OnInit } from '@angular/core';
import { NgbDateStruct, NgbDate, NgbCalendar, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-datepicker-popup',
  templateUrl: './datepicker.component.html',
  styleUrls: ['./datepicker.component.css']
})
export class NgbdDatepickerPopup {
  @Input() public setToday: boolean
  model: NgbDateStruct;

  constructor(private calendar: NgbCalendar, public formatter: NgbDateParserFormatter)
  {

  }

  ngOnInit(): void
  {
    if (this.setToday) {
      this.model = this.calendar.getToday();
    }
  }
}




