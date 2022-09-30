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

  getBoxingOptions(): Observable<any[]>
  {//processResult只會回傳包裝過的json result.content，回傳值給model沒有用。
    const url = `/Common/GetBoxingOptions`;
    const options = this.generatePostOptions();

    return this.httpClient
      .get<IResultDto<any[]>>(url, options)
      .pipe(
        tap((_) => this.log('getBoxingOptions'))
        ,map((result) => this.processResult(result))
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
}
