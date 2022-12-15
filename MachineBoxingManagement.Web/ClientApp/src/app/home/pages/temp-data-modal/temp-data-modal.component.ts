import { SelectionModel } from '@angular/cdk/collections';
import { Component, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { BoxOutService } from '../../../core/http/box-out.service';
import { ReportService } from '../../../core/http/report.service';
import { SweetalertService } from '../../../core/services/sweetalert.service';
import { PartNumber_Model_Desc } from '../../../shared/models/dto/response/box-in';
import { LocalStorageKey } from '../../../shared/models/localstorage-model';
import * as ls from "local-storage";

@Component({
  selector: 'app-temp-data-modal',
  templateUrl: './temp-data-modal.component.html',
  styleUrls: ['./temp-data-modal.component.css']
})
export class TempDataModalComponent implements OnInit {

  displayedColumns = ['Favorite', 'SerialNo', 'PartNumber', 'BoxingName', 'BoxingSerial', 'StackLevel', 'Model', 'Description'];
  dataSource = new MatTableDataSource<PartNumber_Model_Desc>();
  isLoading: boolean;
  selection_Favorite = new SelectionModel<PartNumber_Model_Desc>(true, []);

  constructor
  (
    private _boxOutService: BoxOutService,
    private _reportService: ReportService,
    private _swlService: SweetalertService,
    public activeModal: NgbActiveModal
  )
  { }

  ngOnInit(): void
  {
    this.dataSource.data = ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.myFavorite);
    this.selection_Favorite = new SelectionModel<PartNumber_Model_Desc>(true, this.dataSource.data);    //checkbox default value
  }

  /*匯出暫存資料*/
  exportTempData()
  {
    let tempData = ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.myFavorite);

    if (tempData.length <= 0) {
      this._swlService.showSwal("", `查無暫存資料`, "warning");
      return;
    }

    this._reportService.exportTempData(tempData).toPromise()
      .catch(
        err => {
          this._swlService.showSwal("", `無法產生Excel.<br\>錯誤訊息：${err.message}`, "warning");
        });
  }

  //匯出裝箱暫存資料
  exportFavoriteData()
  {
    let tempData = ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.myFavorite);

    if (tempData.length <= 0) {
      this._swlService.showSwal("", `查無暫存資料`, "warning");
      return;
    }

    this._reportService.exportFavoriteData(tempData).toPromise()
      .catch(
        err => {
          this._swlService.showSwal("", `無法產生Excel.<br\>錯誤訊息：${err.message}`, "warning");
        });
  }

  /*[暫存] checkbox*/
  isAllSelected_Favorite() {
    const numSelected = this.selection_Favorite.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  /*[暫存全選] checkbox*/
  masterToggle_Favorite() {
    this.isAllSelected_Favorite() ?
      this.selection_Favorite.clear() :
      this.dataSource.data.forEach(row => this.selection_Favorite.select(row));
  }

  closeModal()
  {
    ls.set<PartNumber_Model_Desc[]>(LocalStorageKey.myFavorite, this.selection_Favorite.selected)
    this.activeModal.close('Close click');
  }

}
