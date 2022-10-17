import { Component, OnInit, Input, OnChanges, HostListener, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, FormControl, Validators } from '@angular/forms';
import { NgbDate, NgbDatepicker, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';
import { BoxOutService } from '../../../core/http/box-out.service';
import { CommonService } from '../../../core/http/common.service';
import { LocalStorageService } from '../../../core/services/local-storage.service';
import { SoundPlayService } from '../../../core/services/sound-play.service';
import { SweetalertService } from '../../../core/services/sweetalert.service';
import { ITakeOutPostMessageDto } from '../../../shared/models/dto/takeout-postmessage-dto';
import { CheckBoxList } from '../../../shared/models/dto/response/check-box-list';
import { TempDataModalComponent } from '../temp-data-modal/temp-data-modal.component';
import { NgbdDatepickerPopup } from '../../../shared/datepicker/datepicker.component';
import { boxOutQueryCondition } from '../../../shared/models/dto/request/box-out-query-condition';

@Component({
  selector: 'app-box-out',
  templateUrl: './box-out.component.html',
  styleUrls: ['./box-out.component.css']
})
export class BoxOutComponent implements OnInit, OnChanges {
  @Input() activeNo: string;
  @Input() inputUserName: string;
  @ViewChild("txt_sd_boxin") sdBoxIn: NgbdDatepickerPopup;
  @ViewChild("txt_ed_boxin") edBoxIn: NgbdDatepickerPopup;
  @ViewChild("txt_sd_boxout") sdBoxOut: NgbdDatepickerPopup;
  @ViewChild("txt_ed_boxout") edBoxOut: NgbdDatepickerPopup;

  locations: CheckBoxList[] = [];
  options: CheckBoxList[] = [];
  styles: CheckBoxList[] = [];
  statuses: CheckBoxList[] = [];
  bufferAreas: CheckBoxList[] = [];
  form: FormGroup;
  isLoading: boolean = false;
  Swal = Swal;
  previewWindow: Window; // 記錄開啟的 window 物件
  ls_key_take_out_modal_open: string = "isTakeOutModalOpen";
  ls_key_take_out_modal_data_condition: string = "takeOutModalQueryCondition";
  ls_key_list_data: string = "temp_list_data";
  currentUserName: string = "";
  setToday: boolean = true;

  constructor(
    private _modalService: NgbModal,
    private _swlService: SweetalertService,
    private _commonService: CommonService,
    private _boxoutService: BoxOutService,    
    private fb: FormBuilder,
    private _soundPlayService: SoundPlayService,
    private _localStorageService: LocalStorageService
  ) {

    let allCheckDefaultValue: boolean;
    let conditionJsonString = this._localStorageService.getLocalStorageData(this.ls_key_take_out_modal_data_condition);
    if (conditionJsonString == "undefined")
    {
      allCheckDefaultValue = true;
    }
    else
    {
      allCheckDefaultValue = JSON.parse(conditionJsonString)["all_ckb_list_option"];
    }

    this.form = this.fb.group({
      txt_pn: new FormControl(""),//90NB0S51-T00070, 90NB0TE3-E00040
      txt_model: new FormControl(""),//UX482EG
      ckb_list_location: this.fb.array([]),
      all_ckb_list_option: new FormControl(allCheckDefaultValue),
      ckb_list_option: this.fb.array([]),
      ckb_list_style: this.fb.array([]),
      ckb_list_status: this.fb.array([]),
      ckb_list_bufferArea: this.fb.array([])
    })
  }

  async ngOnInit()
  {
    this.isLoading = true;

    /*checkbox設定*/
    try
    {
      this.locations = await this._commonService.getBoxingLocations().toPromise();
      this.options = await this._commonService.getBoxingOptions().toPromise();
      this.styles = await this._commonService.getBoxingStyle().toPromise();
      this.statuses = await this._commonService.getBoxingStatus().toPromise();
      this.bufferAreas = [{ id: 1, name: "非暫存區", checked: true }, { id: 2, name: "暫存區", checked: true }]

      let arrayControlName: string[] = ["ckb_list_location", "ckb_list_option", "ckb_list_style", "ckb_list_status", "ckb_list_bufferArea"];
      let arrayConditionPropertyName: string[] = ["locations", "options", "styles", "statuses", "buffer_areas"];
      let arrayCheckBoxs: CheckBoxList[][] = [this.locations, this.options, this.styles, this.statuses, this.bufferAreas];

      if (this._localStorageService.getLocalStorageData(this.ls_key_take_out_modal_data_condition) != "undefined")
      {
        let condition: boxOutQueryCondition = JSON.parse(this._localStorageService.getLocalStorageData(this.ls_key_take_out_modal_data_condition));

        //設定初始值
        this.form.controls["txt_pn"].setValue(condition.pn);
        this.form.controls["txt_model"].setValue(condition.model);        
        this.sdBoxIn.model = condition.take_in_dt_s ? this.ngbDatePaser(condition.take_in_dt_s) : undefined;
        this.edBoxIn.model = condition.take_in_dt_e ? this.ngbDatePaser(condition.take_in_dt_e) : undefined;
        this.sdBoxOut.model = condition.take_out_dt_s ? this.ngbDatePaser(condition.take_out_dt_s) : undefined;
        this.edBoxOut.model = condition.take_out_dt_e ? this.ngbDatePaser(condition.take_out_dt_e) : undefined;
        this.form.controls["all_ckb_list_option"].setValue(condition.all_ckb_list_option);

        for (var i = 0; i < arrayControlName.length; i++)
        {
          const arrayData = this.form.get(arrayControlName[i]) as FormArray;

            for (var j = 0; j < arrayCheckBoxs[i].length; j++)
            {
              arrayCheckBoxs[i][j].checked = false;

              //從condition物件取得查詢條件內容
              let arrayConditionProperty: number[] = condition[arrayConditionPropertyName[i].toString()];

              for (var k = 0; k < arrayConditionProperty.length; k++)
              {
                if (arrayCheckBoxs[i][j].id == arrayConditionProperty[k])
                {
                  arrayCheckBoxs[i][j].checked = true;
                  arrayData.push(new FormControl(arrayCheckBoxs[i][j].id));//更新FormControl
                }
              }
            }
        }
      }
      else
      {//預設checkbox為全部勾選設定
        for (var i = 0; i < arrayControlName.length; i++) {
          const arrayData = this.form.get(arrayControlName[i]) as FormArray;
          for (var j = 0; j < arrayCheckBoxs[i].length; j++) {
            arrayCheckBoxs[i][j].checked = true;
            arrayData.push(new FormControl(arrayCheckBoxs[i][j].id))
          }
        }
      }
    }
    catch (e)
    {
      this.Swal.fire({
        icon: "error",
        text: "初始資料載入失敗，請聯絡專案室.",
        showConfirmButton: false,
        allowOutsideClick: false,//點擊空白處關閉confirm
        allowEscapeKey: false//ESC關閉confirm
      })
    }

    //切換分頁後，this.previewWindow會變成undefinded。如果已經有開啟modal，就要再重新open一次取得previewWindow
    if (this.isTakeOutModalOpen() == "1")
    {
      this.openWindow();
    }
    else {
      this.isLoading = false
    }

  }

  //變更操作者
  ngOnChanges()
  {
    if (this.inputUserName != this.currentUserName)
    {//@Input的inputUserName變更要重新onSubmit()，重新開啟機台取出視窗，取得最新的UserName

      if (this.currentUserName && this.isTakeOutModalOpen() == "1")
      {
        let validateMsg = this.formValidate();
        if (validateMsg.length > 0)
        {
          /*主畫面關閉要通知彈跳視窗關閉*/
          let passData: ITakeOutPostMessageDto = {
            isParentClose: true
          }         
          this.previewWindow.postMessage(passData, `${window.location.origin}/take_out_list`)

          this._swlService.showSwal("", validateMsg, "warning");
        }
        else
        {
          this.onSubmit();
        }
      }

      this.currentUserName = this.inputUserName;
    }
  }

  //查詢條件勾選
  onCheckBoxChange(e, arrayName: string)
  {
    let i: number = 0;
    const dataArray: FormArray = this.form.get(arrayName) as FormArray;    

    if (e.target.checked)
    {
      dataArray.push(new FormControl(e.target.value));
    }
    else
    {
      dataArray.controls.forEach((item: FormControl) => {
        if (item.value == e.target.value) {
          dataArray.removeAt(i);
          return;
        }

        i++;
      })
    }
  }

  //查詢條件(全部)勾選或取消
  onAllCheckBoxChange(e, arrayName: string)
  {
    const dataArray: FormArray = this.form.get(arrayName) as FormArray;

    let arrayLength = dataArray.length;
    for (var i = arrayLength - 1; i >= 0; i--)
    {
      dataArray.removeAt(i);
    }

    this.options.forEach((item) => {

      if (e.target.checked)
      {//全部選取
        item.checked = true;
        dataArray.push(new FormControl(item.id));
      }
      else
      {//全部選取
        item.checked = false;
      }
    })
  }

  //送出查詢
  onSubmit()
  {
    this.isLoading = true;
    let validateMsg = this.formValidate();//取得檢核結果

    if (validateMsg.length > 0)
    {
      this._swlService.showSwal("", validateMsg, "warning");
      this.isLoading = false;      
    }
    else
    {
      let querycondition: boxOutQueryCondition =
      {
        pn: this.form.controls["txt_pn"].value,
        model: this.form.controls["txt_model"].value,
        take_in_dt_s: this.dateParser(this.sdBoxIn),
        take_in_dt_e: this.dateParser(this.edBoxIn),
        take_out_dt_s: this.dateParser(this.sdBoxOut),
        take_out_dt_e: this.dateParser(this.edBoxOut),
        all_ckb_list_option: this.form.controls["all_ckb_list_option"].value,
        locations: this.form.get('ckb_list_location').value,
        options: this.form.get('ckb_list_option').value,
        styles: this.form.get('ckb_list_style').value,
        statuses: this.form.get('ckb_list_status').value,
        buffer_areas: this.form.get('ckb_list_bufferArea').value
      };

      this._localStorageService.setLocalStorageData(this.ls_key_take_out_modal_data_condition, querycondition);

      this.openWindow();
    }
  }

  //查看暫存資料
  queryFavorites(): void
  {
    let favorites = this._boxoutService.getFavoritesId();

    if (favorites && favorites.length > 0)
    {
      const modalRef = this._modalService.open(TempDataModalComponent, { size: 'xl', backdrop: 'static' });

      //結束(關閉)彈跳視窗後更新取出清單
      modalRef.result
        .then(() => {
          if (this.isTakeOutModalOpen() == "1")
          {           
            let passData: ITakeOutPostMessageDto = {
              favoriteChange: true
            }
            this.previewWindow.postMessage(passData, `${window.location.origin}/take_out_list`)
          }

        },
        (error) => {
          // on error/dismiss
        });
    }
    else {
      this._swlService.showSwal("", "查無資料", "warning");
    }

  }

  isTakeOutModalOpen(): string {
    return this._localStorageService.getLocalStorageData(this.ls_key_take_out_modal_open);
  }

  openWindow(): void
  {
    /*
     * popup parameters setting :https://javascript.info/popup-windows
     */
    this.previewWindow = window.open(`take_out_list/${this.inputUserName}`, "Take Out Machine", "width=800,height=850");
    this._localStorageService.setLocalStorageData(this.ls_key_take_out_modal_open, 1)
  }

  //接收機台清單彈跳視窗回傳是否查詢完畢訊號，解除isLoading遮罩
  @HostListener('window:message', ['$event'])
  onMessage(event: MessageEvent): void
  {
    if (event.data.isLoading != undefined)
    {
      this.isLoading = event.data.isLoading;              
    }

    //if (event.data.queryFavorites != undefined)
    //{
    //  this.queryFavorites();
    //}    
  }

  //轉換日期格視為yyyy/MM/dd字串
  dateParser(data: NgbdDatepickerPopup): string
  {
    let res: string = "";
    if (data && data.model)
    {
      let d = new Date(`${data.model.year}/${data.model.month}/${data.model.day}`);
      if (d.toDateString() != "Invalid Date")
      {
        res = `${data.model.year}/${data.model.month.toString().padStart(2, '0')}/${data.model.day.toString().padStart(2, '0')}`;
      }
      else
      {
        res = "Invalid Date";
      }
    }
    return res;
  }

  ngbDatePaser(d: string): NgbDate
  {
    return new NgbDate(Number(d.substring(0, 4)), Number(d.substring(5, 7)), Number(d.substring(9, 10)));
  }

  //輸入查詢條件檢核
  formValidate(): string
  {
    let pn = this.form.controls["txt_pn"].value;
    let model = this.form.controls["txt_model"].value;
    let takeInDateStart = this.dateParser(this.sdBoxIn);
    let takeInDateEnd = this.dateParser(this.edBoxIn);
    let takeOutDateStart = this.dateParser(this.sdBoxOut);
    let takeOutDateEnd = this.dateParser(this.edBoxOut);

    let msg: string = "";

    if (!pn && !model) {
      msg += "請先輸入查詢條件，P/N與Model必須擇一填寫.</br></br>";
    }

    let aryDateInfo: string[] = [takeInDateStart, takeInDateEnd, takeOutDateStart, takeOutDateEnd];
    let aryDateInfoCN: string[] = ["入庫日期(起)", "入庫日期(迄)", "取出日期(起)", "取出日期(迄)"];

    for (var i = 0; i < aryDateInfo.length; i++)
    {
      if (aryDateInfo[i] == "Invalid Date")
      {
        msg += `${aryDateInfoCN[i]}日期格式錯誤.<//br></br>`;
      }
    }

    return msg;
  }
}
