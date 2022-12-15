import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { tap, map } from 'rxjs/operators';
import { CheckBoxList } from '../../shared/models/dto/response/check-box-list';
import { IResultDto } from '../../shared/models/dto/result-dto';


@Injectable({
  providedIn: 'root'
})
export class CommonService extends BaseService {

  constructor(
    private httpClient: HttpClient
  )
  {
    super();
  }

  getUserName(): Observable<string> {
    const url = `/Common/GetUserName`;
    const options = this.generatePostOptions();

    return this.httpClient
      .get<IResultDto<string>>(url, options)
      .pipe(
        tap((_) => this.log('getUserName'))
        , map((result) => this.processResult(result))
      );
  }

  getBoxingLocations(): Observable<CheckBoxList[]> {
    const url = `/Common/GetBoxingLocations`;
    const options = this.generatePostOptions();

    return this.httpClient
      .get<IResultDto<CheckBoxList[]>> (url, options)
      .pipe(
        tap((_) => this.log('getBoxingLocations'))
        ,map((result) => this.processResult(result))
      );
  }

  getBoxingOptions(): Observable<CheckBoxList[]>
  {//processResult只會回傳包裝過的json result.content，回傳值給model沒有用。
    const url = `/Common/GetBoxingOptions`;
    const options = this.generatePostOptions();

    return this.httpClient
      .get<IResultDto<CheckBoxList[]>>(url, options)
      .pipe(
        tap((_) => this.log('getBoxingOptions'))
        , map((res) =>  this.processResult(res) )
      );
  }

  getBoxingStyle(): Observable<any[]> {
    const url = `/Common/GetBoxingStyle`;
    const options = this.generatePostOptions();

    return this.httpClient
      .get<IResultDto<any[]>>(url, options)
      .pipe(
        tap((_) => this.log('getBoxingStyle'))
        , map((result) => this.processResult(result))
      );
  }

  getBoxingStatus(): Observable<any[]> {
    const url = `/Common/GetBoxingStatus`;
    const options = this.generatePostOptions();

    return this.httpClient
      .get<IResultDto<any[]>>(url, options)
      .pipe(
        tap((_) => this.log('getBoxingStatus'))
        , map((result) => this.processResult(result))
      );
  }

  //取得指定部門(系統-PC-產品管理中心-專案管理一處-管理一部)的人員清單
  getEmployeeInfo(): Observable<IResultDto<any>> {
    const url = `/Common/GetEmployeeInfo`;
    const options = this.generatePostOptions();

    return this.httpClient
      .get<IResultDto<any>>(url, options)
      .pipe(
        tap((_) => this.log('GetEmployeeInfo'))
        //, map((result) => this.processResult(result))
      );
  }

  //檢察部門權限，僅供[系統-PC-產品管理中心-專案管理一處-管理一部]使用
  DeptCheck(): Observable<IResultDto<boolean>>
  {
    const url = `/Common/DeptCheck`;
    const options = this.generatePostOptions();

    return this.httpClient
      .get<IResultDto<boolean>>(url, options)
      .pipe(
        tap((_) => this.log('DeptCheck'))
        //,map((result) => this.processResult(result))
      );
  }
}
