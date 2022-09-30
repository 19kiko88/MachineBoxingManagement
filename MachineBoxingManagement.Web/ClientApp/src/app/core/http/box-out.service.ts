import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from './base.service';
import { PartNumber_Model_Desc, BoxinProcessingData } from '../../shared/models/dto/response/box-in';
import { IResultDto } from '../../shared/models/dto/result-dto';
import { tap, map } from 'rxjs/operators';
import { boxOutItem } from '../../shared/models/dto/request/box-out';
import { LocalStorageService } from '../services/local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class BoxOutService extends BaseService {

  constructor
  (
    private httpClient: HttpClient,
    private _localStorageService: LocalStorageService
  ) {
    super();
  }

  //取出維護-查詢
  queryMachines(partNumber: string, model: string, location: number[], option: number[], style: number[], status: number[], bufferArea: boolean, favorites: number[]): Observable<IResultDto<PartNumber_Model_Desc[]>>
  {
    let paraLocation = "";
    let paraOption = "";
    let paraStyle = "";
    let paraStatus = "";

    if (location.length > 0) {
      for (var i = 0; i < location.length; i++) {
        paraLocation += `&locations=${location[i]}`;
      }
    }

    if (option.length > 0) {
      for (var i = 0; i < option.length; i++) {
        paraOption += `&options=${option[i]}`;
      }
    }

    if (style.length > 0) {
      for (var i = 0; i < style.length; i++) {
        paraStyle += `&styles=${style[i]}`;
      }
    }

    if (status.length > 0) {
      for (var i = 0; i < status.length; i++) {
        paraStatus += `&statuses=${status[i]}`;
      }
    }

    if (favorites.length > 0) {
      for (var i = 0; i < favorites.length; i++) {
        paraStatus += `&favorites=${favorites[i]}`;
      }
    }

    const url = `/BoxOut/QueryMachines?pn=${partNumber}&model=${model}${paraLocation}${paraOption}${paraStyle}${paraStatus}&buffer_area=${bufferArea}`;
    const options = this.generatePostOptions();

    return this.httpClient
      .get<IResultDto<PartNumber_Model_Desc[]>>(url, options)
      .pipe(
        tap(() => {
          this.log("execute api QueryMachines.");
          console.log("execute api QueryMachines.");
        })
      )
  }

  //取出維護-取出機台
  takeoutMachines(userName: string, ids: number[]): Observable<IResultDto<any>> {
    const url = `/BoxOut/TakeoutMachines`;
    const options = this.generatePostOptions();

    let boxOutItem: boxOutItem[] = [];
    let params = { userName: userName, ids: ids, boxOut_Item: boxOutItem };

    return this.httpClient
      .post<IResultDto<any>>(url, params, options)
      .pipe(
        tap(() => {
          this.log("saveBoxingInfos")
        })
      )
  }


  //取出維護-更新暫存區
  saveMachineBufferArea(userName: string, boxOutItem: boxOutItem[]): Observable<IResultDto<number[]>>
  {
    const url = `/BoxOut/SaveMachineBufferArea`;
    const options = this.generatePostOptions();

    let ids: number[] = [];
    let params = { userName: userName, ids: ids, boxOut_Item: boxOutItem };

    return this.httpClient
      .post<IResultDto<number[]>>(url, params, options)
      .pipe(
        tap(() => {
          this.log("SaveMachineBufferArea")
        })
      )
  }

  //取得暫存資料的id
  getFavoritesId(): number[]
  {
    let favorites: number[] = [];

    if (this._localStorageService.getLocalStorageData("my_favorite") != "undefined") {
      let arrayFavorites = JSON.parse(this._localStorageService.getLocalStorageData("my_favorite"))
      for (var i = 0; i < arrayFavorites.length; i++) {
        favorites.push(arrayFavorites[i].id);
      }
    }

    return favorites;
  }
}
