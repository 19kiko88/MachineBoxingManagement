import { Component, OnInit, Input, OnChanges, HostListener } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, FormControl, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import Swal from 'sweetalert2';
import { BoxOutService } from '../../../core/http/box-out.service';
import { CommonService } from '../../../core/http/common.service';
import { LocalStorageService } from '../../../core/services/local-storage.service';
import { SoundPlayService } from '../../../core/services/sound-play.service';
import { SweetalertService } from '../../../core/services/sweetalert.service';
import { ITakeOutPostMessageDto } from '../../../shared/models/dto/takeout-postmessage-dto';
import { CheckBoxList } from '../../../shared/models/dto/response/check-box-list';
import { Enum_Sound } from '../../../shared/models/enum/sound';
import { TempDataModalComponent } from '../temp-data-modal/temp-data-modal.component';

@Component({
  selector: 'app-box-out',
  templateUrl: './box-out.component.html',
  styleUrls: ['./box-out.component.css']
})
export class BoxOutComponent implements OnInit, OnChanges {
  @Input() activeNo: string;
  @Input() inputUserName: string; 

  locations: CheckBoxList[] = [];
  options: CheckBoxList[] = [];
  style: CheckBoxList[] = [];
  statuses: CheckBoxList[] = [];
  form: FormGroup;
  isLoading: boolean = false;
  Swal = Swal;
  previewWindow: Window; // 記錄開啟的 window 物件
  ls_key_take_out_modal_open: string = "isTakeOutModalOpen";
  ls_key_take_out_modal_data_condition: string = "takeOutModalQueryCondition";
  ls_key_list_data: string = "temp_list_data";
  currentUserName: string = "";

  constructor(
    private _modalService: NgbModal,
    private _swlService: SweetalertService,
    private _commonService: CommonService,
    private _boxoutService: BoxOutService,    
    private fb: FormBuilder,
    private _soundPlayService: SoundPlayService,
    private _localStorageService: LocalStorageService
  ) {
    this.form = this.fb.group({
      txt_pn: new FormControl(""),//90NB0S51-T00070, 90NB0TE3-E00040
      txt_model: new FormControl(""),//UX482EG
      locationArray: this.fb.array([]),
      optionArray: this.fb.array([]),
      styleArray: this.fb.array([]),
      statusArray: this.fb.array([]),
      chk_buffer_area: new FormControl(true),
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
      this.style = await this._commonService.getBoxingStyle().toPromise();
      this.statuses = await this._commonService.getBoxingStatus().toPromise();

      let arrayControlName: string[] = ["locationArray", "optionArray", "styleArray", "statusArray"];
      let arrayCheckBoxs: CheckBoxList[][] = [this.locations, this.options, this.style, this.statuses];

      if (this._localStorageService.getLocalStorageData(this.ls_key_take_out_modal_data_condition) != "undefined")
      {
        let condition = JSON.parse(this._localStorageService.getLocalStorageData(this.ls_key_take_out_modal_data_condition));

        this.form.controls["txt_pn"].setValue(condition.pn);
        this.form.controls["txt_model"].setValue(condition.model);
        this.form.controls["chk_buffer_area"].setValue(condition.bufferArea);

        for (var i = 0; i < arrayControlName.length; i++)
        {
          const arrayData = this.form.get(arrayControlName[i]) as FormArray;

            for (var j = 0; j < arrayCheckBoxs[i].length; j++)
            {
              arrayCheckBoxs[i][j].checked = false;

              switch (arrayControlName[i])
              {//checkbox是否打勾設定
                case "locationArray":
                  for (var k = 0; k < condition.locations.length; k++)
                  {
                    if (arrayCheckBoxs[i][j].id == condition.locations[k])
                    {
                      arrayCheckBoxs[i][j].checked = true;
                      arrayData.push(new FormControl(arrayCheckBoxs[i][j].id));//更新FormControl
                    }
                  }
                  break;
                case "optionArray":
                  for (var k = 0; k < condition.options.length; k++)
                  {
                    if (arrayCheckBoxs[i][j].id == condition.options[k])
                    {
                      arrayCheckBoxs[i][j].checked = true;
                      arrayData.push(new FormControl(arrayCheckBoxs[i][j].id));//更新FormControl
                    }
                  }
                  break;
                case "styleArray":
                  for (var k = 0; k < condition.styles.length; k++) {
                    if (arrayCheckBoxs[i][j].id == condition.styles[k])
                    {
                      arrayCheckBoxs[i][j].checked = true;
                      arrayData.push(new FormControl(arrayCheckBoxs[i][j].id));//更新FormControl
                    }
                  }
                  break;
                case "statusArray":
                  for (var k = 0; k < condition.statuses.length; k++) {
                    if (arrayCheckBoxs[i][j].id == condition.statuses[k])
                    {
                      arrayCheckBoxs[i][j].checked = true;
                      arrayData.push(new FormControl(arrayCheckBoxs[i][j].id));//更新FormControl
                    }
                  }
                  break;
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
    } catch (e)
    {
      this.Swal.fire({
        icon: "error",
        text: "初始資料載入失敗，請聯絡專案室.",
        showConfirmButton: false,
        allowOutsideClick: false,//點擊空白處關閉confirm
        allowEscapeKey: false//ESC關閉confirm
      })
    }

    /*主畫面關閉要通知彈跳視窗關閉*/
    let passData: ITakeOutPostMessageDto = {
      isParentClose: true
    }
    window.addEventListener("beforeunload", (event) => {
      this.previewWindow.postMessage(passData, `${window.location.origin}/take_out_list`)
    }, false);

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
        this.onSubmit(false);
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

  //送出查詢
  onSubmit(showConditionAlert: boolean = true)
  {
    this.isLoading = true;
    let pn = this.form.controls["txt_pn"].value;
    let model = this.form.controls["txt_model"].value;
    let locations = this.form.get('locationArray').value;
    let options = this.form.get('optionArray').value;
    let styles = this.form.get('styleArray').value;
    let statuses = this.form.get('statusArray').value;
    let bufferArea = this.form.controls["chk_buffer_area"].value;

    if (!pn && !model)
    {
      if (showConditionAlert)
      {
        this._swlService.showSwal("", "請先輸入查詢條件<br>P/N與Model必須擇一填寫.", "warning");
      }

      this.isLoading = false;
      return;
    }

    this.openWindow();
    let querycondition = { pn: pn, model: model, locations: locations, options: options, styles: styles, statuses: statuses, bufferArea: bufferArea };
    this._localStorageService.setLocalStorageData(this.ls_key_take_out_modal_data_condition, querycondition);
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


  /**
 * 接收機台清單彈跳視窗回傳是否查詢完畢訊號，解除isLoading遮罩
 * @param event
 */
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
}
