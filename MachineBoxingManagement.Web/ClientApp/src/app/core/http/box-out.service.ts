import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseService } from './base.service';
import { IResultDto } from '../../shared/models/dto/result-dto';
import { tap, map } from 'rxjs/operators';
import { boxOutItem } from '../../shared/models/dto/request/box-out';
import { boxOutQueryCondition } from '../../shared/models/dto/request/box-out-query-condition';
import { LocalStorageKey } from '../../shared/models/localstorage-model';
import * as ls from "local-storage";
import { PartNumber_Model_Desc } from '../../shared/models/dto/response/box-in';


@Injectable({
  providedIn: 'root'
})
export class BoxOutService extends BaseService {

  constructor
  (
    private httpClient: HttpClient
  ) {
    super();
  }

  //取出維護-查詢
  queryMachines(pageIndex: number, pageSize: number, data: boxOutQueryCondition, favorites: number[]): Observable<IResultDto<any>>
  {
    const url = `/BoxOut/QueryMachines?pageIndex=${pageIndex}&pageSize=${pageSize}`;
    const options = this.generatePostOptions();

    data.favorites = favorites;

    return this.httpClient
      .post<IResultDto<any>>(url, data, options)
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
    const arrayFavorites = ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.myFavorite);

    if (arrayFavorites.length > 0)
    {
      for (var i = 0; i < arrayFavorites.length; i++)
      {
        favorites.push(arrayFavorites[i].id);
      }
    }

    return favorites;
  }
}
