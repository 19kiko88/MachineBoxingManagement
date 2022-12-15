import { SelectionModel } from '@angular/cdk/collections';
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { BoxOutService } from '../../../core/http/box-out.service';
import { ReportService } from '../../../core/http/report.service';
import { SweetalertService } from '../../../core/services/sweetalert.service';
import { PartNumber_Model_Desc } from '../../../shared/models/dto/response/box-in';
import { ITakeOutPostMessageDto } from '../../../shared/models/dto/takeout-postmessage-dto'
import { ActivatedRoute } from '@angular/router';
import { SoundPlayService } from '../../../core/services/sound-play.service';
import { Enum_Sound } from '../../../shared/models/enum/sound';
import { boxOutItem } from '../../../shared/models/dto/request/box-out';
import { switchMap, tap } from 'rxjs/operators';
import { boxOutQueryCondition } from '../../../shared/models/dto/request/box-out-query-condition';
import { MatPaginator, MatPaginatorIntl, PageEvent } from '@angular/material/paginator';
import { LocalStorageKey } from '../../../shared/models/localstorage-model';
import * as ls from "local-storage";
import * as uuid from 'uuid';



@Component({
  selector: 'app-take-out-modal',
  templateUrl: './take-out-modal.component.html',
  styleUrls: ['./take-out-modal.component.css']
})

export class TakeOutModalComponent implements OnInit {

  inputUserName: string;
  inputTempDatas: PartNumber_Model_Desc[] = [];
  inputTempDatasLength: number = 0;

  displayedColumns = ['Favorite', 'SerialNo', 'select', 'PartNumber', 'select_BufferArea', 'BoxingName', 'BoxingSerial', 'StackLevel', 'Model', 'Description', 'InStockDate', 'Location', 'BoxingOption', 'Operator', 'OperateTime', 'TakeOutOperator', 'TakeOutOperateTime' /*'SSN'*/];
  dataSource = new MatTableDataSource<PartNumber_Model_Desc>();
  selection = new SelectionModel<PartNumber_Model_Desc>(true, []);
  selection_BufferArea = new SelectionModel<PartNumber_Model_Desc>(true, []);
  selection_Favorite = new SelectionModel<PartNumber_Model_Desc>(true, []);
  isLoading: boolean = false;
  passData: ITakeOutPostMessageDto = { inputUserName: "", inputData: null, isParentClose: false }
  refreshMain: boolean = false;
  favorites: PartNumber_Model_Desc[] = ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.myFavorite);
  @ViewChild('paginator') paginator: MatPaginator;
  pageEvent: PageEvent;
  idx: number = 0;

  constructor(
    private _route: ActivatedRoute,
    private _reportService: ReportService,
    private _swlService: SweetalertService,
    private _boxOutService: BoxOutService,
    private _soundPlayService: SoundPlayService,
    private matPaginatorIntl: MatPaginatorIntl
  ) { }

  ngOnInit(): void
  {
    this.inputUserName = this._route.snapshot.params["user_name"];//第一次進彈跳視窗先從url取得user_name，之後主畫面改操作者會透過posetMessage更新
    const defaultUUID = this._route.snapshot.params["uuid"];

    if (defaultUUID != ls.get<uuid>(LocalStorageKey.uuid))
    {
      this._swlService.showSwalNoButtonConfirm("", "請由MBM主頁進入.", "error");
      return;
    }

    //查詢機台
    this.queryMachines();

    //監聽視窗關閉事件
    window.addEventListener("beforeunload", () => {
      ls.set<number>(LocalStorageKey.isTakeOutModalOpen, 0);//0:彈跳視窗關閉, 1:彈跳視窗開啟
    }, false);

    /*監測關閉modal*/
    var intervalId = window.setInterval(() => {
      if (
        !ls.get<string>(LocalStorageKey.operators)//操作者清空後，沒有操作者就關閉modal。
        ||!ls.get<string>(LocalStorageKey.jwt)//jwt過期或沒有jwt，關閉modal。
        ||ls.get<number>(LocalStorageKey.isTakeOutModalOpen) == -1//切換到裝箱維護分頁時，強制關閉取出維護modal
      )
      {
        clearInterval(intervalId);//結束interval
        window.close();
      }
    }, 1000)

    // 設定顯示筆數資訊文字
    this.matPaginatorIntl.getRangeLabel = (page: number, pageSize: number, length: number): string => {
      if (length === 0 || pageSize === 0) {
        return `第 0 筆、共 ${length} 筆`;
      }

      length = Math.max(length, 0);
      const startIndex = page * pageSize;
      const endIndex = startIndex < length ? Math.min(startIndex + pageSize, length) : startIndex + pageSize;

      return `第 ${startIndex + 1} - ${endIndex} 筆、共 ${length} 筆`;
    };

    // 設定其他顯示資訊文字
    this.matPaginatorIntl.itemsPerPageLabel = '每頁筆數：';
    this.matPaginatorIntl.nextPageLabel = '下一頁';
    this.matPaginatorIntl.previousPageLabel = '上一頁';

    ls.set<PartNumber_Model_Desc[]>(LocalStorageKey.bufferAreas, []);
  }

  /*監聽主畫面傳來的取出資訊*/
  @HostListener('window:message', ['$event'])
  onMessage(event: MessageEvent<ITakeOutPostMessageDto>): void
  {
    //主畫面關閉，取出彈跳視窗跟著關閉
    if (event.data.isParentClose)
    {
      ls.set<number>(LocalStorageKey.isTakeOutModalOpen, 0);//0:彈跳視窗關閉, 1:彈跳視窗開啟
      window.close();
    }

    //變更查詢條件
    if (event.data.inputData)
    {
      this.inputUserName = event.data.inputUserName;
      this.inputTempDatas = event.data.inputData;
      this.dataSource.data = event.data.inputData;
    }

    //變更暫存資料
    if (event.data.favoriteChange)
    {
      this.queryMachines();
    }

  }

  /*[取出] checkbox*/
  isAllSelected()
  {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /*[取出全選] checkbox*/
  masterToggle()
  {
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

    this.dataSource.data.forEach(item => {      
      for (var i = 0; i < this.favorites.length; i++)
      {
        if (this.favorites[i].id == item.id)
        {
          this.favorites.splice(i, 1);
          break;
        }
      }
    });

    if (isAllSelected$)
    {//全部取消暫存
      this.selection_Favorite.clear();
    }
    else
    {//全部暫存
      this.dataSource.data.forEach(item => {
        this.favorites.push(item);
        this.selection_Favorite.select(item);
      });
    }

    ls.set<PartNumber_Model_Desc[]>(LocalStorageKey.myFavorite, this.favorites);
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
            console.log("take out process chain done.");

            this._swlService.showSwalConfirm("", resTotal, "info",
              () => {
                window.opener.postMessage({ resetOperator: true }, `${window.location.origin}`);//通知主畫面清空操作者
                window.close();
              },null, null, null, false
            )
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
  queryMachines(pageIndex: number = 0, pageSize: number = 500, exportExcel: boolean = false): void
  {
    this.isLoading = true;
    this.selection = new SelectionModel<PartNumber_Model_Desc>(true, []);

    let condition: boxOutQueryCondition = ls.get<boxOutQueryCondition>(LocalStorageKey.takeOutModalQueryCondition);
    this._boxOutService.queryMachines(pageIndex, pageSize, condition, this._boxOutService.getFavoritesId())
      .subscribe(
        (res) => {
          if (res.message) {
            this._soundPlayService.playSound(Enum_Sound.Error);
            this._swlService.showSwal("", `錯誤：<br\>${res.message}`, "error");
            this.isLoading = false;
            return;
          }

          if (res.content.item1.length > 0)
          {
            this.inputTempDatas = res.content.item1;
            this.dataSource.data = res.content.item1;
            this.inputTempDatasLength = res.content.item2;//總筆數

            //設定暫存checkbox是否勾選
            this.selection_Favorite = new SelectionModel<PartNumber_Model_Desc>(true, this.dataSource.data.filter(c => c.is_Favorite == true));

            //設定暫存區checkbox是否勾選
            const bufferAreas: PartNumber_Model_Desc[] = ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.bufferAreas);
            bufferAreas.forEach(c => {
              this.changeBufferArea(c, true);
            })

            //匯出Excel
            if (exportExcel)
            {
              this._reportService.exportTempData(this.inputTempDatas).toPromise()
                .catch(
                  err => {
                    this._swlService.showSwal("", `無法產生Excel.<br\>錯誤訊息：${err.message}`, "warning");
                  });
            }
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
  exportTempData()
  {
    this.queryMachines(0, 65535, true);
  }

  /*勾選暫存區checkbox事件*/
  changeBufferArea(data: PartNumber_Model_Desc, executeByQuery = false)
  {
    this.isLoading = true;
    let bufferAreas: PartNumber_Model_Desc[] = ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.bufferAreas);
    const item_is_Buffer_Area = !data.is_Buffer_Area;

    //手動更改checkbox時，要把同廠區. 箱名. 箱號的資訊存放在LocalStorage，切換其他分頁時自動判斷暫存區checkbox是否勾選
    if (executeByQuery == false)
    {
      let idx = bufferAreas.findIndex(c => c.boxing_Location_Id === data.boxing_Location_Id && c.boxing_Series === data.boxing_Series && c.boxing_Serial === data.boxing_Serial)
      if (item_is_Buffer_Area == true)
      {
        if (idx < 0)
        {
          bufferAreas.push(data);
        }
      }
      else
      {
        bufferAreas.splice(idx, 1);
      }

      ls.set<PartNumber_Model_Desc[]>(LocalStorageKey.bufferAreas, bufferAreas);
    }


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

    var favorite_checked = this.selection_Favorite.isSelected(data);
    if (favorite_checked)
    {
      this.favorites.push(data);
    }
    else
    {
      let idx = this.favorites.findIndex(c => c.id == data.id);
      this.favorites.splice(idx, 1);
    }

    ls.set<PartNumber_Model_Desc[]>(LocalStorageKey.myFavorite, this.favorites);
  }

  // 分頁切換時，重新取得資料
  handlePageEvent(e: PageEvent)
  {
    this.pageEvent = e;
    this.queryMachines(e.pageIndex, e.pageSize);
    this.idx = (e.pageIndex * e.pageSize);
  }
}
