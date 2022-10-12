import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from './base.service';
import { PartNumber_Model_Desc, BoxinProcessingData } from '../../shared/models/dto/response/box-in';
import { IResultDto } from '../../shared/models/dto/result-dto';
import { tap, map } from 'rxjs/operators';
import { boxOutItem } from '../../shared/models/dto/request/box-out';
import { LocalStorageService } from '../services/local-storage.service';
import { boxOutQueryCondition } from '../../shared/models/dto/request/box-out-query-condition';

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
  queryMachines(data: boxOutQueryCondition, favorites: number[]): Observable<IResultDto<PartNumber_Model_Desc[]>>
  {
    let paraLocation = "";
    let paraOption = "";
    let paraStyle = "";
    let paraStatus = "";
    let paraBufferAreas = "";

    if (data.locations.length > 0) {
      for (var i = 0; i < data.locations.length; i++) {
        paraLocation += `&locations=${data.locations[i]}`;
      }
    }

    if (data.options.length > 0) {
      for (var i = 0; i < data.options.length; i++) {
        paraOption += `&options=${data.options[i]}`;
      }
    }

    if (data.styles.length > 0) {
      for (var i = 0; i < data.styles.length; i++) {
        paraStyle += `&styles=${data.styles[i]}`;
      }
    }

    if (data.statuses.length > 0) {
      for (var i = 0; i < data.statuses.length; i++) {
        paraStatus += `&statuses=${data.statuses[i]}`;
      }
    }

    if (data.buffer_areas.length > 0) {
      for (var i = 0; i < data.buffer_areas.length; i++) {
        paraBufferAreas += `&buffer_area=${data.buffer_areas[i]}`;
      }
    }

    if (favorites.length > 0) {
      for (var i = 0; i < favorites.length; i++) {
        paraStatus += `&favorites=${favorites[i]}`;
      }
    }

    let conditionDate: string = `&s_takeInDt=${data.take_in_dt_s}&e_takeInDt=${data.take_in_dt_e}&s_takeOutDt=${data.take_out_dt_s}&e_takeOutDt=${data.take_out_dt_e}`;

    const url = `/BoxOut/QueryMachines?pn=${data.pn}&model=${data.model}${conditionDate}${paraLocation}${paraOption}${paraStyle}${paraStatus}${paraBufferAreas}`;
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
