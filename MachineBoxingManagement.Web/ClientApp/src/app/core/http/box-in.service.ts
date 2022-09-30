import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { tap, map } from 'rxjs/operators';
import { IResultDto } from '../../shared/models/dto/result-dto';
import { BoxinProcessingData, PartNumber_Model_Desc } from '../../shared/models/dto/response/box-in';

@Injectable({
  providedIn: 'root'
})
export class BoxInService extends BaseService {

  processingData: BoxinProcessingData = {
    saveTemp: false,
    partNumber: "",
    location: 1,
    machineOption: 1,
    machineStyle: 1,
    boxSeries: "",
    boxSerial: 1,
    ssn: "",
    pnDatas: []
  };

  constructor(
    private httpClient: HttpClient
  ) { super(); }

  ProcessingPN(userName: string, saveTemp: boolean, partNumber: string, location: number, machineOption: number, machineStyle: number, ssn: string, tempDataList: PartNumber_Model_Desc[]): Observable<any> {
    const url = `/BoxIn/ProcessingPN`;

    this.processingData.userName = userName;
    this.processingData.saveTemp = saveTemp;
    this.processingData.partNumber = partNumber;
    this.processingData.location = location;
    this.processingData.machineOption = machineOption;
    this.processingData.machineStyle = machineStyle;
    this.processingData.pnDatas = tempDataList;

    const options = this.generatePostOptions();
    return this.httpClient
      .post<IResultDto<any>>(url, this.processingData, options)
      .pipe(
        tap((_) => this.log('processingPN'))
        //, map((result) => this.processResult(result))
      );
  }

  //裝箱維護(主畫面)-取得箱號 & 儲位相關資訊
  GetStockingInfo(boxingSeries: string, location: number, saveTemp: boolean, tempDataList: PartNumber_Model_Desc[]): Observable<any> {
    const url = `/BoxIn/Get_Stocking_Info/${boxingSeries}`;
    const options = this.generatePostOptions();

    this.processingData.saveTemp = saveTemp;
    this.processingData.location = location;
    this.processingData.pnDatas = tempDataList;

    return this.httpClient
      .post<IResultDto<any>>(url, this.processingData, options)
      .pipe(
        tap((_) => this.log('GetStockingInfo'))
        //, map((result) => this.processResult(result))
      );
  }

  //裝箱維護(Modal)-取得指定箱號 & 儲位相關資訊
  getStockingInfoByBoxSerial(boxingSeries: string, location: number, boxSerial: number, tempDataList: PartNumber_Model_Desc[]): Observable<IResultDto<any>>
  {
    const url = `/BoxIn/Get_Machine_In_Box_Info_By_BoxSerial`;
    const options = this.generatePostOptions();

    this.processingData.boxSeries = boxingSeries;
    this.processingData.location = location;
    this.processingData.boxSerial = boxSerial;
    this.processingData.pnDatas = tempDataList;

    return this.httpClient
      .post<IResultDto<any>>(url, this.processingData, options)
      .pipe(
        tap((_) => this.log('Get_Machine_In_Box_Info_By_BoxSerial'))
        //, map((result) => this.processResult(result))
      );
  }

  //裝箱維護-批次儲存
  saveBoxingInfos(data: PartNumber_Model_Desc[]): Observable<IResultDto<number>> {
    const url = `/BoxIn/SaveBoxingInfos`;
    const options = this.generatePostOptions();

    return this.httpClient
      .post<IResultDto<number>>(url, data, options)
      .pipe(
        tap(() => {
          this.log("saveBoxingInfos")
        })
      )
  }
}
