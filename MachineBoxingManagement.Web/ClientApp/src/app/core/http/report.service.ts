import { HttpClient } from '@angular/common/http';
import { Injectable, LOCALE_ID, Inject } from '@angular/core';
import { BaseService } from './base.service';
import { catchError, map, tap } from 'rxjs/operators';
import { PartNumber_Model_Desc } from '../../shared/models/dto/response/box-in';
import { Observable } from 'rxjs';
import { IResultDto } from '../../shared/models/dto/result-dto';
import { formatDate } from '@angular/common';
//import 'rxjs/add/operator/catch';
//import { of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReportService extends BaseService {

  constructor(
    @Inject(LOCALE_ID) public locale: string,
    private httpClient: HttpClient) {
    super();
  }

  exportTempData(data: PartNumber_Model_Desc[]) {
    const url = `/Report/Export_Temp_Data`;
    const options: any = this.generateGetOptions();
    options.responseType = 'arraybuffer';

    return this.httpClient.post(url, data, options).pipe(
      tap((_) => this.log('Export_Sticker')),
      map((data) =>
        this.downloadFile(
          `機台資訊_${formatDate(Date.now(), 'yyyyMMddHHmmss', this.locale)}.xlsx`,
          data,
          'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        )
      )
    );
  }

  exportFavoriteData(data: PartNumber_Model_Desc[]) {
    const url = `/Report/Export_Favorite_Data`;
    const options: any = this.generateGetOptions();
    options.responseType = 'arraybuffer';

    return this.httpClient.post(url, data, options).pipe(
      tap((_) => this.log('Export_Sticker')),
      map((data) =>
        this.downloadFile(
          `暫存機台資訊_${formatDate(Date.now(), 'yyyyMMddHHmmss', this.locale)}.xlsx`,
          data,
          'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        )
      )
    );
  }

  exportSticker(data: PartNumber_Model_Desc[]): Observable<any> {    
    const url = `/Report/Export_Sticker`;
    const options: any = this.generateGetOptions();
    options.responseType = 'arraybuffer';

    return this.httpClient.post(url, data, options).pipe(
      tap((_) => this.log('Export_Sticker')),
      map((data) =>        
        this.downloadFile(
          `外箱貼紙_${formatDate(Date.now(), 'yyyyMMddHHmmss', this.locale)}.xlsx`,
          data,
          'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
        )
      //),
      //catchError(err => {
        //  return of(`exportSticker err! ${err.message}`);}
      )
    );
  }

  private downloadFile(name: string, data: any, type: string) {
    /*Error test code */
    //let qq: number[];
    //qq[0] = 1;
    //let qqq: number = qq[5];
     
    const blob = new Blob([data], { type: type });
    const url = window.URL.createObjectURL(blob);
    var link = document.createElement('a');
    link.href = url;
    link.download = name;
    link.click();
    link.remove();

    // const pwa = window.open(url);
    // if (!pwa || pwa.closed || typeof pwa.closed == 'undefined') {
    //   alert('請允許彈出視窗後重新下載.');
    // }
  }
}
