import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { AppModule } from '../../app.module';


@Component({
  selector: 'app-counter-plus-minus',
  templateUrl: './counter-plus-minus.component.html',
  styleUrls: ['./counter-plus-minus.component.css']
})
export class CounterPlusMinusComponent implements OnInit {

  /*  @Input() Label_Type?: string;*/
  @Output() current_value: EventEmitter<number> = new EventEmitter<number>();
  @Input() inputMinValue: number;
  @Input() inputMaxValue: number;

  value: number = 0;

  constructor() { }

  ngOnInit() {
    //設定預設值
    this.value = this.inputMinValue;
  }

  handleMinus() {
    if (this.value > this.inputMinValue)
    {
      //小於預設值就不再-1
      this.value--;
      this.current_value.emit(this.value);
    }
  }

  handlePlus()
  {
    if (this.value < this.inputMaxValue) {
      this.value++;
      this.current_value.emit(this.value);    
    }
  }
}
