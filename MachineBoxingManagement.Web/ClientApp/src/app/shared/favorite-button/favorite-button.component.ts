import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { LocalStorageService } from '../../core/services/local-storage.service';
import { PartNumber_Model_Desc } from '../models/dto/response/box-in'

@Component({
  selector: 'app-favorite-button',
  templateUrl: './favorite-button.component.html',
  styleUrls: ['./favorite-button.component.css']
})
export class FavoriteButtonComponent implements OnInit {

  @Input() selected: boolean;
  @Input() inputTakeOutElement: any;// PartNumber_Model_Desc;
  @Output() selectedChange = new EventEmitter<boolean>();

  constructor(private _localStorageService: LocalStorageService) { }

  ngOnInit() {
  }

  public toggleSelected()
  {
    this.selected = !this.selected;

    /*** 觸發事件 ***/
    let arrayFavorite: PartNumber_Model_Desc[] = [];
    if (this._localStorageService.getLocalStorageData("my_favorite") == "undefined")
    {
      this._localStorageService.setLocalStorageData("my_favorite", []);
    }
    else
    {
      arrayFavorite = JSON.parse(this._localStorageService.getLocalStorageData("my_favorite"));
    }

    if (this.selected == true)
    {//選取
      this.inputTakeOutElement.is_Favorite = true;
      arrayFavorite.push(this.inputTakeOutElement);
    }
    else
    {//取消選取
      let idx: number = arrayFavorite.findIndex(c => c.id == this.inputTakeOutElement.id);
      arrayFavorite.splice(idx, 1);
    }
    this._localStorageService.setLocalStorageData("my_favorite", arrayFavorite);
    /*** ******** ***/

    this.selectedChange.emit(this.selected);
  }

}
