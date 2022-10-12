import { SelectionModel } from '@angular/cdk/collections';
import { Component, HostListener, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { BoxOutService } from '../../../core/http/box-out.service';
import { ReportService } from '../../../core/http/report.service';
import { LocalStorageService } from '../../../core/services/local-storage.service';
import { SweetalertService } from '../../../core/services/sweetalert.service';
import { PartNumber_Model_Desc } from '../../../shared/models/dto/response/box-in';
import { ITakeOutPostMessageDto } from '../../../shared/models/dto/takeout-postmessage-dto'
import { ActivatedRoute } from '@angular/router';
import { SoundPlayService } from '../../../core/services/sound-play.service';
import { Enum_Sound } from '../../../shared/models/enum/sound';
import { boxOutItem } from '../../../shared/models/dto/request/box-out';
import { catchError, map, switchMap, tap } from 'rxjs/operators';
import { IResultDto } from '../../../shared/models/dto/result-dto';
import { observable, throwError } from 'rxjs';
import { boxOutQueryCondition } from '../../../shared/models/dto/request/box-out-query-condition';



@Component({
  selector: 'app-take-out-modal',
  templateUrl: './take-out-modal.component.html',
  styleUrls: ['./take-out-modal.component.css']
})

export class TakeOutModalComponent implements OnInit {

  inputUserName: string;
  inputTempDatas: PartNumber_Model_Desc[] = [];

  tempDatasBackup: PartNumber_Model_Desc[] = [];
  displayedColumns = ['Favorite', 'SerialNo', 'select', 'PartNumber', 'select_BufferArea', 'BoxingName', 'BoxingSerial', 'StackLevel', 'Model', 'Description', 'InStockDate', 'Location', 'BoxingOption', 'Operator', 'OperateTime', 'TakeOutOperator', 'TakeOutOperateTime' /*'SSN'*/];
  dataSource = new MatTableDataSource<PartNumber_Model_Desc>();
  selection = new SelectionModel<PartNumber_Model_Desc>(true, []);
  selection_BufferArea = new SelectionModel<PartNumber_Model_Desc>(true, []);
  selection_Favorite = new SelectionModel<PartNumber_Model_Desc>(true, []);
  isLoading: boolean = false;
  ls_key_list_data: string = "temp_list_data";
  ls_key_take_out_modal_data_condition: string = "takeOutModalQueryCondition";
  passData: ITakeOutPostMessageDto = { inputUserName: "", inputData: null, isParentClose: false }
  refreshMain: boolean = false;

  constructor(
    private _route: ActivatedRoute,
    private _reportService: ReportService,
    private _swlService: SweetalertService,
    private _boxOutService: BoxOutService,
    private _localStorageService: LocalStorageService,
    private _soundPlayService: SoundPlayService
  ) { }

  ngOnInit(): void
  {
    this.inputUserName = this._route.snapshot.params["user_name"];//第一次進彈跳視窗先從url取得user_name，之後主畫面改操作者會透過posetMessage更新

    //查詢機台
    this.queryMachines();

    //監聽視窗關閉事件
    window.addEventListener("beforeunload", () => {
      this._localStorageService.setLocalStorageData("isTakeOutModalOpen", 0);//0:彈跳視窗關閉, 1:彈跳視窗開啟
    }, false);
  }

  /*監聽主畫面傳來的取出資訊*/
  @HostListener('window:message', ['$event'])
  onMessage(event: MessageEvent<ITakeOutPostMessageDto>): void
  {
    //主畫面關閉，取出彈跳視窗跟著關閉
    if (event.data.isParentClose)
    {
      this._localStorageService.setLocalStorageData("isTakeOutModalOpen", 0);//0:彈跳視窗關閉, 1:彈跳視窗開啟
      window.close();
    }

    //變更查詢條件
    if (event.data.inputData)
    {
      this.inputUserName = event.data.inputUserName;
      this.inputTempDatas = event.data.inputData;
      this.dataSource.data = event.data.inputData;
      this.tempDatasBackup = event.data.inputData
    }

    //變更暫存資料
    if (event.data.favoriteChange)
    {
      this.queryMachines();
    }

  }

  /*[取出] checkbox*/
  isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /*[取出全選] checkbox*/
  masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  /*[暫存] checkbox*/
  isAllSelected_Favorite()
  {
    const numSelected = this.selection_Favorite.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /*[暫存全選] checkbox*/
  masterToggle_Favorite()
  {
    let isAllSelected$ = this.isAllSelected_Favorite();
    if (isAllSelected$)
    {//全部取消暫存
      this.selection_Favorite.clear();
      this._localStorageService.setLocalStorageData("my_favorite", []);
    }
    else
    {//全部暫存
      this.dataSource.data.forEach(row => this.selection_Favorite.select(row));
      this._localStorageService.setLocalStorageData("my_favorite", this.inputTempDatas);
    }
  }

  /*變更暫存區狀態 & 取出機台(先變更暫存區狀態後取出，因為取出會把機台移出array會導致更新機台暫存狀態undefinded出錯)*/
  saveChange()
  {
    this._swlService.showSwalConfirm(
      "",
      `確定要儲存變更?`,
      "warning",
      //confirm callback function
      () => {
        this.isLoading = true;//彈跳視窗loading遮罩
        window.opener.postMessage({ isLoading: this.isLoading }, `${window.location.origin}`);//主畫面loading遮罩

        //取得要取出的機台ID
        let resTotal: string = "";
        let arrayId: number[] = [];
        let selection = this.selection.selected.filter(c => c.status_Id == 666);//篩選666，避免重複取出
        if (selection.length > 0) {
          for (var i = 0; i < selection.length; i++) {
            arrayId.push(selection[i].id);
          }
        }

        let arrayBoxOutItem: boxOutItem[] = [];
        let selection_BufferArea = this.selection_BufferArea.selected;
        //取得要更新暫存狀態的機台資訊
        for (var i = 0; i < selection_BufferArea.length; i++) {
          let boxOutItem: boxOutItem = { id: 0, isBufferArea: false };
          boxOutItem.id = selection_BufferArea[i].id;
          boxOutItem.isBufferArea = selection_BufferArea[i].is_Buffer_Area;
          arrayBoxOutItem.push(boxOutItem);
        }

        console.log("take out process chain go.");
        this._boxOutService.saveMachineBufferArea(this.inputUserName, arrayBoxOutItem).pipe(
          tap(resFromSaveBufferArea => {
            console.log("take out process chain[saveMachineBufferArea] go.");
            if (resFromSaveBufferArea.success) {
              for (var i = 0; i < arrayBoxOutItem.length; i++)
              {
                this.inputTempDatas.find(c => c.id == arrayBoxOutItem[i].id).is_Buffer_Area = arrayBoxOutItem[i].isBufferArea;
              }

              //清空selection_BufferArea
              this.selection_BufferArea = new SelectionModel<PartNumber_Model_Desc>(true, []);
              resTotal += `總共異動暫存區筆數：${selection_BufferArea.length}<br\>成功筆數：${resFromSaveBufferArea.content.length}(含不同料號)<br\>錯誤訊息：${resFromSaveBufferArea.message}<br\><br\>`;
            }
            else
            {
              resTotal += `總共異動暫存區失敗：${resFromSaveBufferArea.message}<br\><br\>`;
            }
            console.log("take out process chain[saveMachineBufferArea] done.");
          }),
          switchMap(() =>
            this._boxOutService.takeoutMachines(this.inputUserName, arrayId)
          ),
          tap((resFromTakeOut) => {
            console.log("take out process chain[takeoutMachines] go.");
            if (resFromTakeOut.success) {
              for (var i = 0; i < resFromTakeOut.content.length; i++) {
                let takeoutData: PartNumber_Model_Desc = this.inputTempDatas.find(c => c.id == resFromTakeOut.content[i]);
                let item_idx = this.inputTempDatas.indexOf(takeoutData);
                this.inputTempDatas.splice(item_idx, 1);
              }
              //清空selection
              this.selection = new SelectionModel<PartNumber_Model_Desc>(true, []);
              resTotal += `總共取出筆數：${selection.length}<br\>成功筆數：${resFromTakeOut.content.length}<br\>錯誤訊息：${resFromTakeOut.message}<br\><br\>`;
            }
            else
            {
              resTotal += `總共取出失敗：${resFromTakeOut.message}<br\><br\>`;
            }
            console.log("take out process chain[takeoutMachines] done.");
          })
        ).subscribe(
          (res) => {//next
            this.isLoading = false;
            this._swlService.showSwal("", resTotal, "info");
            this.queryMachines();//查詢機台
            console.log("take out process chain done.");
          },
          (err) => {//error
            this.isLoading = false;
            this._swlService.showSwal("", `異動失敗，請聯繫CAE Team：<br\>${err}`, "error");
            console.log(err);
          }
        )      
      });
  }

  /*機台查詢*/
  queryMachines(): void
  {
    this.isLoading = true;

    let condition: boxOutQueryCondition = JSON.parse(this._localStorageService.getLocalStorageData(this.ls_key_take_out_modal_data_condition));
    this._boxOutService.queryMachines(condition, this._boxOutService.getFavoritesId())
      .subscribe(
        (res) => {
          if (res.message) {
            this._soundPlayService.playSound(Enum_Sound.Error);
            this._swlService.showSwal("", `錯誤：<br\>${res.message}`, "error");
            this.isLoading = false;
            return;
          }

          if (res.content.length > 0)
          {
            this.inputTempDatas = res.content;
            this.dataSource.data = res.content;
            //checkbox default value
            this.selection_Favorite = new SelectionModel<PartNumber_Model_Desc>(true, this.dataSource.data.filter(c => c.is_Favorite == true));
            this.tempDatasBackup = JSON.parse(JSON.stringify(this.inputTempDatas));//避免傳址參考跟著變動，用json轉物件做備份供資料恢復
          }
          else {            
            this._swlService.showSwal("", "查無資料", "warning");
          }
          this.isLoading = false;
        },
        (err) => {
          this.isLoading = false;
        },
        () => {
          this.isLoading = false;

          //主畫面增加loading遮罩
          window.opener.postMessage({ isLoading: this.isLoading }, `${window.location.origin}`);
        }
      )
  }

  /*匯出暫存資料*/
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

  /*勾選暫存區checkbox事件*/
  changeBufferArea(data: PartNumber_Model_Desc)
  {
    this.isLoading = true;
    let item_is_Buffer_Area = !data.is_Buffer_Area;

    for (var i = 0; i < this.inputTempDatas.length; i++)
    {
      if (this.inputTempDatas[i].status_Id == 666 && this.inputTempDatas[i].boxing_Location_Id === data.boxing_Location_Id && this.inputTempDatas[i].boxing_Series === data.boxing_Series && this.inputTempDatas[i].boxing_Serial === data.boxing_Serial)
      {//同Location. 箱名. 箱號的機台要一起被勾選
        this.inputTempDatas[i].is_Buffer_Area = item_is_Buffer_Area;
        this.selection_BufferArea.toggle(this.inputTempDatas[i]);
      }
    }

    this.isLoading = false;
  }

  /*暫存勾選事件*/
  changeFavorite(data: PartNumber_Model_Desc)
  {
    this.selection_Favorite.toggle(data);
    this._localStorageService.setLocalStorageData("my_favorite", this.selection_Favorite.selected);
    //window.opener.postMessage({ queryFavorites: true }, `${window.location.origin}`);
  }
}
