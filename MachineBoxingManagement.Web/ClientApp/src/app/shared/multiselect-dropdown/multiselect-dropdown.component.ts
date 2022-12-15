import { Component, Input, OnInit, Output, EventEmitter, OnChanges } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { IDropdownSettings } from 'ng-multiselect-dropdown';

@Component({
  selector: 'app-multiselect-dropdown',
  templateUrl: './multiselect-dropdown.component.html',
  styleUrls: ['./multiselect-dropdown.component.css']
})

/*
 *ng-multiselect-dropdown API Manual：
 * https://www.npmjs.com/package/ng-multiselect-dropdown
 * 
 * */

export class MultiselectDropdownComponent implements OnInit, OnChanges {

  constructor() { }

  @Input() inputOptionList;
  @Output() outputOptionList: EventEmitter<any> = new EventEmitter<any>();

  dropdownList = [];
  selectedItems = [];
  dropdownSettings: IDropdownSettings = {};

  ngOnChanges()
  {
    this.dropdownList = this.inputOptionList;
  }

  ngOnInit() {
    this.dropdownList = this.inputOptionList;

    /*Default Option*/
    //this.selectedItems = [{ item_id: 1, item_text: 'Test' }];

    this.dropdownSettings = {
      singleSelection: false,
      idField: 'item_id',
      textField: 'item_text',
      selectAllText: '全選',
      unSelectAllText: '取消全選',
      itemsShowLimit: 3,
      allowSearchFilter: true,
      searchPlaceholderText: '搜尋...'
    };

  }

  onItemSelect(item: any) {
    this.outputOptionList.emit(this.selectedItems);
  }
  onItemDeSelect(item: any) {
    this.outputOptionList.emit(this.selectedItems);
  }
  onSelectAll(items: any) {
    this.outputOptionList.emit(items);
  }
  onDeSelectAll(items: any) {
    this.outputOptionList.emit(items);
  }
}
