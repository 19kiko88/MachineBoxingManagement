import { SelectionModel } from '@angular/cdk/collections';
import { Component, HostListener, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { BoxInService } from '../../../core/http/box-in.service';
import { BoxOutService } from '../../../core/http/box-out.service';
import { ReportService } from '../../../core/http/report.service';
import { LocalStorageService } from '../../../core/services/local-storage.service';
import { SweetalertService } from '../../../core/services/sweetalert.service';
import { PartNumber_Model_Desc } from '../../../shared/models/dto/response/box-in';
import { ITakeInPostMessageDto } from '../../../shared/models/dto/takein-postmessage-dto'
import { ActivatedRoute } from '@angular/router';
import { SoundPlayService } from '../../../core/services/sound-play.service';
import { Enum_Sound } from '../../../shared/models/enum/sound';

@Component({
  selector: 'app-take-in-modal',
  templateUrl: './take-in-modal.component.html',
  styleUrls: ['./take-in-modal.component.css']
})
export class TempListModalComponent implements OnInit {

  inputUserName: string;
  inputTempDatas: PartNumber_Model_Desc[] = [];

  tempDatasBackup: PartNumber_Model_Desc[] = [];
  displayedColumns = ['SerialNo', 'select', 'PartNumber', 'select_BufferArea', 'BoxingName', 'BoxingSerial', 'StackLevel', 'Model', 'Description', 'InStockDate', 'Location', 'BoxingOption', 'Operator', 'OperateTime' /*'SSN'*/];
  dataSource = new MatTableDataSource<PartNumber_Model_Desc>();
  selection = new SelectionModel<PartNumber_Model_Desc>(true, []);
  selection_BufferArea = new SelectionModel<PartNumber_Model_Desc>(true, []);
  isLoading: boolean = false;
  selectType: string = "刪除?";
  ls_key_list_data: string = "temp_list_data";
  passData: ITakeInPostMessageDto = { inputUserName: "", inputData: null, isParentClose: false }
  refreshMain: boolean = false;
  lastRow: number = 0;
  isFullBox: boolean;
  alarmMsg: string;
  isBlink: boolean;

  constructor(
    private _route: ActivatedRoute,
    private _reportService: ReportService,
    private _swlService: SweetalertService,
    private _boxInService: BoxInService,
    private _localStorageService: LocalStorageService,
    private _soundPlayService: SoundPlayService
  ) { }

  ngOnInit(): void
  {
    let userName = this._route.snapshot.params["user_name"];

    if (this._localStorageService.getLocalStorageData(this.ls_key_list_data, JSON.stringify([])) != "undefined")
    {
      this.inputUserName = userName;//第一次進彈跳視窗先從url取得user_name，之後主畫面改操作者會透過posetMessage更新
      this.inputTempDatas = JSON.parse(this._localStorageService.getLocalStorageData(this.ls_key_list_data));
      this.dataSource.data = this.inputTempDatas;
      this.tempDatasBackup = JSON.parse(JSON.stringify(this.inputTempDatas));//避免傳址參考跟著變動，用json轉物件做備份供資料恢復
      this.lastRow = this.inputTempDatas.length - 1;
    }

    /*監聽視窗關閉事件 */
    window.addEventListener("beforeunload", () => {
      this._localStorageService.setLocalStorageData("isTakeInModalOpen", 0);//0:彈跳視窗關閉, 1:彈跳視窗開啟
    }, false);
  }

  /*監聽主畫面傳來的機台資訊*/
  @HostListener('window:message', ['$event'])
  onMessage(event: MessageEvent<ITakeInPostMessageDto>): void
  {
    //主畫面關閉，裝箱彈跳視窗跟著關閉
    if (event.data.isParentClose) {
      this._localStorageService.setLocalStorageData("isTakeInModalOpen", 0);//0:彈跳視窗關閉, 1:彈跳視窗開啟
      window.close();
    }
    
    if (event.data.inputData)
    {
      this.isFullBox = false;
      this.alarmMsg = "";
      this.isBlink = false;
      this.inputUserName = event.data.inputUserName;
      this.inputTempDatas = JSON.parse(this._localStorageService.getLocalStorageData(this.ls_key_list_data));
      this.dataSource.data = this.inputTempDatas;
      this.tempDatasBackup = JSON.parse(JSON.stringify(this.inputTempDatas));//避免傳址參考跟著變動，用json轉物件做備份供資料恢復

      /*定錨最後一列*/
      let chatHistory = document.getElementById("temp-table");
      chatHistory.scrollTop = chatHistory.scrollHeight;

      this.lastRow = this.inputTempDatas.length - 1;


      this._boxInService.getStockingInfoByBoxSerial(event.data.inputData.boxing_Series, event.data.inputData.boxing_Location_Id, event.data.inputData.boxing_Serial, this.inputTempDatas).subscribe(
        res => {
          let data = res.content;
          if (data.qty == 20)
          {//over 20 pcs
            this.isBlink = true;
            this.alarmMsg = `箱號：${event.data.inputData.boxing_Serial}。箱內機台數已滿上限!`
            this.isFullBox = true;
            this._soundPlayService.playSound(Enum_Sound.Alarm)//play alarm audio
          }
          else {
            this.isBlink = false;
            this.alarmMsg = `箱號${data.box_Serial}，箱內數量總計：${data.qty} (資料庫已有數量：${data.boxing_Db_Qty}，暫存筆數數量：${data.boxing_Temp_Qty})`
          }
        },
        err => {

        }
      )
    }
  }

  /*回傳更新StockingInfo通知給主畫面*/
  passRefreshParentSignal()
  {
    this.refreshMain = true;
    window.opener.postMessage(this.refreshMain, `${window.location.origin}`);
    this.refreshMain = false;
  }

  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  isAllSelected_BufferArea() {
    const numSelected = this.selection_BufferArea.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  masterToggle_BufferArea() {
    this.isAllSelected_BufferArea() ?
      this.selection_BufferArea.clear() :
      this.dataSource.data.forEach(row => this.selection_BufferArea.select(row));
  }

  /*
   * 裝箱維護：刪除勾選的暫存資料列
   */
  saveChange() {
    this._swlService.showSwalConfirm(
      "",
      `確定要進行(刪除)操作?`,
      "warning",
      //confirm callback function
      () => {
        this.alarmMsg = "";
        let selected_item = this.selection.selected;

        for (var i = 0; i < selected_item.length; i++) {
          let item_idx = this.inputTempDatas.indexOf(selected_item[i]);
          this.inputTempDatas.splice(item_idx, 1);
        }

        this.dataSource.data = this.inputTempDatas;
        this.setLocalStorage();
        this.selection = new SelectionModel<PartNumber_Model_Desc>(true, []);
        this.passRefreshParentSignal();
      });

    if (this.inputTempDatas.length <= 0) {
      this._swlService.showSwal("", `查無暫存資料`, "warning");
      return;
    }


  }

  //匯出裝箱暫存資料
  exportTempData() {
    if (this.inputTempDatas.length <= 0) {
      this._swlService.showSwal("", `查無暫存資料`, "warning");
      return;
    }

    this._reportService.exportTempData(this.inputTempDatas).toPromise()
      .catch(
        err => {
          this._swlService.showSwal("", `無法產生Excel.<br\>錯誤訊息：${err.message}`, "warning");
        });
  }

  //winform UnifyStackLevel。手動變更[第幾層]欄位，重新計算[第幾層]欄位。同地點.箱名.箱號的暫存資料第幾層欄位必須一致
  unifyStackLevel(turtleLevel: number, location: number, boxSeries: string, boxSerial: number, sn?: number) {
    this.isLoading = true;

    if (turtleLevel > 5) {
      this._swlService.showSwal("", "最大層數為5", "error");

      //恢復資料
      for (var i = 0; i < this.inputTempDatas.length; i++) {
        if (this.inputTempDatas[i].boxing_Location_Id === location && this.inputTempDatas[i].boxing_Series === boxSeries && this.inputTempDatas[i].boxing_Serial === boxSerial) {
          this.inputTempDatas[i].turtle_Level = this.tempDatasBackup.find(c => c.serial_No === sn).turtle_Level;
        }
      }

      this.isLoading = false;
      return;
    }


    this._swlService.showSwalConfirm(
      "",
      "確認變更同地點.箱名.箱號之所有暫存資料[第幾層]欄位?",
      "warning",
      () => {
        //是，更新[第幾層] & 更新localStorage資料
        for (var i = 0; i < this.inputTempDatas.length; i++) {
          if (this.inputTempDatas[i].boxing_Location_Id === location && this.inputTempDatas[i].boxing_Series === boxSeries && this.inputTempDatas[i].boxing_Serial === boxSerial) {
            this.inputTempDatas[i].turtle_Level = turtleLevel;
          }
        }

        this.tempDatasBackup = JSON.parse(JSON.stringify(this.inputTempDatas));//避免傳址參考跟著變動，用json轉物件做備份供資料恢復

        this.setLocalStorage();        
      },
      () => {
        //否，恢復資料
        for (var i = 0; i < this.inputTempDatas.length; i++) {
          if (this.inputTempDatas[i].boxing_Location_Id === location && this.inputTempDatas[i].boxing_Series === boxSeries && this.inputTempDatas[i].boxing_Serial === boxSerial) {
            this.inputTempDatas[i].turtle_Level = this.tempDatasBackup.find(c => c.serial_No === sn).turtle_Level;
          }
        }
      }
    )

    this.isLoading = false;
  }

  //手動變更[箱號]欄位，重新計算儲位相關資訊
  changeSerial(location: number, boxSeries: string, boxSerial: number, is_Buffer_Area: boolean, sn?: number) {
    let oldBoxSerial = this.tempDatasBackup.find(c => c.serial_No === sn).boxing_Serial;
    let oldTurtleLevel = this.tempDatasBackup.find(c => c.serial_No === sn).turtle_Level;
    let oldis_Buffer_Area = this.tempDatasBackup.find(c => c.serial_No === sn).is_Buffer_Area;

    if (!(boxSerial >= 1 && boxSerial <= 99999))
    {
      //恢復資料
      this.inputTempDatas.find(c => c.serial_No === sn).boxing_Serial = oldBoxSerial;
      this._swlService.showSwal("", "箱號必需為1~99999", "error");
      return;
    }
    else {
      this.inputTempDatas.find(c => c.serial_No === sn).boxing_Serial = boxSerial;
      this.inputTempDatas.find(c => c.serial_No === sn).turtle_Level = 0;//烏龜車層數先變更為0，表示處理中(processing)資料
    }

    this._boxInService.getStockingInfoByBoxSerial(boxSeries, location, boxSerial, this.inputTempDatas).toPromise()
      .then(res => {
        let data = res.content;
        if (data.qty > 20)
        {
          //恢復資料
          this.inputTempDatas.find(c => c.serial_No === sn).boxing_Serial = oldBoxSerial;
          this.inputTempDatas.find(c => c.serial_No === sn).turtle_Level = oldTurtleLevel;
          this._swlService.showSwal("", `箱號：${boxSerial}<br\>箱內數量：${data.qty}(暫存：${data.boxing_Temp_Qty}. 資料庫：${data.boxing_Db_Qty})<br\>已經超過上限20，請更換其他箱號`, "warning");
        }
        else
        {
          this._swlService.showSwalConfirm(
            "",
            `確定要變更箱號${oldBoxSerial} => ${boxSerial}?`,
            "warning",
            () =>
            {
              this.alarmMsg = "";

              //排除掉處理中資料的暫存資料，取得is_Buffer_Area
              let is_Buffer_Area = this.inputTempDatas.find(c => c.turtle_Level != 0/*排除處理中資料*/ && c.boxing_Location_Id == location && c.boxing_Series == boxSeries && c.boxing_Serial == boxSerial)?.is_Buffer_Area;
              if (!is_Buffer_Area)
              {
                is_Buffer_Area = false;
              }

              this.inputTempDatas.find(c => c.serial_No === sn).boxing_Serial = boxSerial;
              this.inputTempDatas.find(c => c.serial_No === sn).turtle_Level = data.turtle_Level;
              this.inputTempDatas.find(c => c.serial_No === sn).is_Buffer_Area = is_Buffer_Area;


              this.tempDatasBackup.find(c => c.serial_No === sn).boxing_Serial = boxSerial;
              this.tempDatasBackup.find(c => c.serial_No === sn).turtle_Level = data.turtle_Level;
              this.tempDatasBackup.find(c => c.serial_No === sn).is_Buffer_Area = is_Buffer_Area;

              this.setLocalStorage();//真正儲存localStorage

              this.passRefreshParentSignal();
            },
            () => {
              //恢復資料
              this.inputTempDatas.find(c => c.serial_No === sn).boxing_Serial = oldBoxSerial;
              this.inputTempDatas.find(c => c.serial_No === sn).turtle_Level = oldTurtleLevel;
              this.inputTempDatas.find(c => c.serial_No === sn).is_Buffer_Area = oldis_Buffer_Area;
            }
          );
        }
      },
        err => {
          //有exception時，恢復資料(ex:api斷掉)
          this.inputTempDatas.find(c => c.serial_No === sn).boxing_Serial = oldBoxSerial;
          this.inputTempDatas.find(c => c.serial_No === sn).turtle_Level = oldTurtleLevel;
          this.inputTempDatas.find(c => c.serial_No === sn).is_Buffer_Area = oldis_Buffer_Area;
        }
      )
  }

  changeBufferArea(data: PartNumber_Model_Desc)
  {
    let item_is_Buffer_Area = !data.is_Buffer_Area;

    for (var i = 0; i < this.inputTempDatas.length; i++) {
      if (this.inputTempDatas[i].boxing_Location_Id === data.boxing_Location_Id && this.inputTempDatas[i].boxing_Series === data.boxing_Series && this.inputTempDatas[i].boxing_Serial === data.boxing_Serial) {
        this.inputTempDatas[i].is_Buffer_Area = item_is_Buffer_Area;
      }
    }

    this.dataSource.data = this.inputTempDatas;
    this.setLocalStorage();
  }

  setLocalStorage() {
    this._localStorageService.setLocalStorageData<PartNumber_Model_Desc[]>(this.ls_key_list_data, this.inputTempDatas);
  }
}
