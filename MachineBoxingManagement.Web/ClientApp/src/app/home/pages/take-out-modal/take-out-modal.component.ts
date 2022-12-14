import { SelectionModel } from '@angular/cdk/collections';
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { BoxOutService } from '../../../core/http/box-out.service';
import { ReportService } from '../../../core/http/report.service';
import { SweetalertService } from '../../../core/services/sweetalert.service';
import { PartNumber_Model_Desc } from '../../../shared/models/dto/response/box-in';
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
import { IPostMessage } from '../../../shared/models/post-message';



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
    this.inputUserName = this._route.snapshot.params["user_name"];//??????????????????????????????url??????user_name???????????????????????????????????????posetMessage??????
    const defaultUUID = this._route.snapshot.params["uuid"];

    if (defaultUUID != ls.get<uuid>(LocalStorageKey.uuid))
    {
      this._swlService.showSwalNoButtonConfirm("", "??????MBM????????????.", "error");
      return;
    }

    //????????????
    this.queryMachines();

    //????????????????????????
    window.addEventListener("beforeunload", () => {
      ls.set<number>(LocalStorageKey.isTakeOutModalOpen, 0);//0:??????????????????, 1:??????????????????
    }, false);

    /*????????????modal*/
    var intervalId = window.setInterval(() => {
      if (
        !ls.get<string>(LocalStorageKey.operators)//?????????????????????????????????????????????modal???
        ||!ls.get<string>(LocalStorageKey.jwt)//jwt???????????????jwt?????????modal???
        ||ls.get<number>(LocalStorageKey.isTakeOutModalOpen) == -1//?????????????????????????????????????????????????????????modal
      )
      {
        clearInterval(intervalId);//??????interval
        window.close();
      }
    }, 1000)

    // ??????????????????????????????
    this.matPaginatorIntl.getRangeLabel = (page: number, pageSize: number, length: number): string => {
      if (length === 0 || pageSize === 0) {
        return `??? 0 ????????? ${length} ???`;
      }

      length = Math.max(length, 0);
      const startIndex = page * pageSize;
      const endIndex = startIndex < length ? Math.min(startIndex + pageSize, length) : startIndex + pageSize;

      return `??? ${startIndex + 1} - ${endIndex} ????????? ${length} ???`;
    };

    // ??????????????????????????????
    this.matPaginatorIntl.itemsPerPageLabel = '???????????????';
    this.matPaginatorIntl.nextPageLabel = '?????????';
    this.matPaginatorIntl.previousPageLabel = '?????????';

    ls.set<PartNumber_Model_Desc[]>(LocalStorageKey.bufferAreas, []);
  }

  /*????????????????????????????????????*/
  @HostListener('window:message', ['$event'])
  onMessage(event: MessageEvent<IPostMessage>): void
  {
    //??????????????????
    if (event.data.boxoutInputDatas)
    {
      this.inputUserName = event.data.inputUserName;
      this.inputTempDatas = event.data.boxoutInputDatas;
      this.dataSource.data = event.data.boxoutInputDatas;
    }

    //??????????????????
    if (event.data.queryMachines)
    {
      this.queryMachines();
    }

  }

  /*[??????] checkbox*/
  isAllSelected()
  {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /*[????????????] checkbox*/
  masterToggle()
  {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  /*[??????] checkbox*/
  isAllSelected_Favorite()
  {
    const numSelected = this.selection_Favorite.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /*[????????????] checkbox*/
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
    {//??????????????????
      this.selection_Favorite.clear();
    }
    else
    {//????????????
      this.dataSource.data.forEach(item => {
        this.favorites.push(item);
        this.selection_Favorite.select(item);
      });
    }

    ls.set<PartNumber_Model_Desc[]>(LocalStorageKey.myFavorite, this.favorites);
  }

  /*????????????????????? & ????????????(??????????????????????????????????????????????????????????????????array?????????????????????????????????undefinded??????)*/
  saveChange()
  {
    this._swlService.showSwalConfirm(
      "",
      `??????????????????????`,
      "warning",
      //confirm callback function
      () => {
        this.isLoading = true;
        const passData: IPostMessage = { isLoading: this.isLoading };//????????????loading??????
        window.opener.postMessage(passData, `${window.location.origin}`);//?????????????????????loading??????

        //????????????????????????ID
        let resTotal: string = "";
        let arrayId: number[] = [];
        let selection = this.selection.selected.filter(c => c.status_Id == 666);//??????666?????????????????????
        if (selection.length > 0) {
          for (var i = 0; i < selection.length; i++) {
            arrayId.push(selection[i].id);
          }
        }

        let arrayBoxOutItem: boxOutItem[] = [];
        let selection_BufferArea = this.selection_BufferArea.selected;
        //??????????????????????????????????????????
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

              //??????selection_BufferArea
              this.selection_BufferArea = new SelectionModel<PartNumber_Model_Desc>(true, []);
              resTotal += `??????????????????????????????${selection_BufferArea.length}<br\>???????????????${resFromSaveBufferArea.content.length}(???????????????)<br\>???????????????${resFromSaveBufferArea.message}<br\><br\>`;
            }
            else
            {
              resTotal += `??????????????????????????????${resFromSaveBufferArea.message}<br\><br\>`;
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
              //??????selection
              this.selection = new SelectionModel<PartNumber_Model_Desc>(true, []);
              resTotal += `?????????????????????${selection.length}<br\>???????????????${resFromTakeOut.content.length}<br\>???????????????${resFromTakeOut.message}<br\><br\>`;
            }
            else
            {
              resTotal += `?????????????????????${resFromTakeOut.message}<br\><br\>`;
            }
            console.log("take out process chain[takeoutMachines] done.");
          })
        ).subscribe(
          (res) => {//next
            this.isLoading = false;
            console.log("take out process chain done.");

            this._swlService.showSwalConfirm("", resTotal, "info",
              () => {
                const passData: IPostMessage = { resetOperator: true };
                window.opener.postMessage(passData, `${window.location.origin}`);//??????????????????????????????
                window.close();
              },null, null, null, false
            )
          },
          (err) => {//error
            this.isLoading = false;
            this._swlService.showSwal("", `????????????????????????CAE Team???<br\>${err}`, "error");
            console.log(err);
          }
        )      
      });
  }

  /*????????????*/
  queryMachines(pageIndex: number = 0, pageSize: number = 500, exportExcel: boolean = false): void
  {
    this.isLoading = true;
    this.selection = new SelectionModel<PartNumber_Model_Desc>(true, []);

    let condition: boxOutQueryCondition = ls.get<boxOutQueryCondition>(LocalStorageKey.takeOutModalQueryCondition);
    this._boxOutService.queryMachines(pageIndex, pageSize, condition, this._boxOutService.getFavoritesId())
      .subscribe(
        (res) =>
        {
          if (res.message) {
            this._soundPlayService.playSound(Enum_Sound.Error);
            this._swlService.showSwal("", `?????????<br\>${res.message}`, "error");
            this.isLoading = false;
            return;
          }

          if (res.content.item1.length > 0)
          {
            this.inputTempDatas = res.content.item1;
            this.dataSource.data = res.content.item1;
            this.inputTempDatasLength = res.content.item2;//?????????

            //????????????checkbox????????????
            this.selection_Favorite = new SelectionModel<PartNumber_Model_Desc>(true, this.dataSource.data.filter(c => c.is_Favorite == true));

            //???????????????checkbox????????????
            const bufferAreas: PartNumber_Model_Desc[] = ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.bufferAreas);
            bufferAreas.forEach(c => {
              this.changeBufferArea(c, true);
            })

            //??????Excel
            if (exportExcel)
            {
              this._reportService.exportTempData(this.inputTempDatas).toPromise()
                .catch(
                  err => {
                    this._swlService.showSwal("", `????????????Excel.<br\>???????????????${err.message}`, "warning");
                  });
            }
          }
          else
          {            
            this._swlService.showSwalConfirm("", "????????????", "warning",
              () => {
                ls.set<number>(LocalStorageKey.isTakeOutModalOpen, -1);
              }
              , null, null, null, false)
          }
        },
        (err) => {
        },
        () => {
          this.isLoading = false;

          //???????????????loading??????
          const passData: IPostMessage = { isLoading: this.isLoading };//????????????loading??????
          window.opener.postMessage(passData, `${window.location.origin}`);
        }
      )
  }

  /*??????????????????*/
  exportTempData()
  {
    this.queryMachines(0, 65535, true);
  }

  /*???????????????checkbox??????*/
  changeBufferArea(data: PartNumber_Model_Desc, executeByQuery = false)
  {
    this.isLoading = true;
    let bufferAreas: PartNumber_Model_Desc[] = ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.bufferAreas);
    const item_is_Buffer_Area = !data.is_Buffer_Area;

    //????????????checkbox?????????????????????. ??????. ????????????????????????LocalStorage?????????????????????????????????????????????checkbox????????????
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
      {//???Location. ??????. ?????????????????????????????????
        this.inputTempDatas[i].is_Buffer_Area = item_is_Buffer_Area;
        this.selection_BufferArea.toggle(this.inputTempDatas[i]);
      }
    }

    this.isLoading = false;
  }

  /*??????????????????*/
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

  // ????????????????????????????????????
  handlePageEvent(e: PageEvent)
  {
    this.pageEvent = e;
    this.queryMachines(e.pageIndex, e.pageSize);
    this.idx = (e.pageIndex * e.pageSize);
  }
}
