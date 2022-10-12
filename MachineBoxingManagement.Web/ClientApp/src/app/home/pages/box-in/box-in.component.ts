import { Component, Input, OnInit, ViewChild, AfterViewInit, ElementRef, HostListener, AfterViewChecked, OnChanges } from '@angular/core';
import { async } from '@angular/core/testing';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DropDowList } from '../../../shared/models/dto/response/drop-down-list';
import { PartNumber_Model_Desc } from '../../../shared/models/dto/response/box-in';
import { Observable } from 'rxjs';
import { CounterPlusMinusComponent } from './../../../shared/counter-plus-minus/counter-plus-minus.component'
import { BoxInService } from '../../../core/http/box-in.service';
import { CommonService } from './../../../core/http/common.service';
import { ReportService } from '../../../core/http/report.service';
import { SweetalertService } from './../../../core/services/sweetalert.service';
import Swal from 'sweetalert2';
import { SoundPlayService } from '../../../core/services/sound-play.service';
import { Enum_Sound } from '../../../shared/models/enum/sound';
import { ITakeInPostMessageDto } from '../../../shared/models/dto/takein-postmessage-dto'
import { LocalStorageService } from '../../../core/services/local-storage.service';
import { IResultDto } from '../../../shared/models/dto/result-dto';

@Component({
  selector: 'app-box-in',
  templateUrl: './box-in.component.html',
  styleUrls: ['./box-in.component.css']
})

export class BoxInComponent implements OnInit, OnChanges {
  @Input() activeNo: string;
  @Input() inputUserName: string;
  @Input() isMultiOpen: boolean;  

  locations: DropDowList[] = [] ;
  options: any[] = [];
  style: any[] = [];
  temp_save: boolean = false;
  pn_desc: string;
  stocking_info_qty: number = -1;
  stocking_info_turtle_level: number = -1;
  stocking_info_qty_desc: string;
  ls_key_list_data: string = "temp_list_data";
  ls_key_take_in_modal_open: string = "isTakeInModalOpen";
  isLoading: boolean = false;
  currentPN: string = "";
  Swal = Swal;
  alarmMsg: string = "";
  previewWindow: Window; // 記錄開啟的 window 物件
  currentUserName: string = "";

  array_pn_model_desc: PartNumber_Model_Desc[] = [];
  pn_model_desc: PartNumber_Model_Desc = {
    serial_No: 0,
    id: 0,
    part_Number: '',
    ssn: '',
    boxing_Location_Id: -1,
    boxing_Location_Cn: '',
    boxing_Option_Id: -1,
    boxing_Option_Cn: '',
    boxing_Series: '',
    boxing_Serial: -1,
    turtle_Level: -1,
    status_Id: -1,
    operator: '',
    operate_Time: null,
    desc: '',
    insDate: null,
    mbResale: null,
    model: '',
    tearDown: false,
    toOA: false,
    is_Buffer_Area: false,
    takeout_Operator: '',
    takeout_Operate_Time: null
  };


  constructor(
    private _commonService: CommonService,
    private _boxInService: BoxInService,
    private _reportService: ReportService,
    private _swlService: SweetalertService,
    private _soundPlayService: SoundPlayService,
    private _localStorageService: LocalStorageService
  )
  { }


  ngOnInit()
  {
    this.GetInitData();

    /*主畫面關閉要通知彈跳視窗關閉*/
    let passData: ITakeInPostMessageDto = {
      isParentClose: true
    }
    window.addEventListener("beforeunload", (event) => {
      this.previewWindow.postMessage(passData, `${window.location.origin}/take_in_list`)
    }, false);


    //切換分頁後，this.previewWindow會變成undefinded。如果已經有開啟modal，就要再重新open一次取得previewWindow
    if (this._localStorageService.getLocalStorageData(this.ls_key_take_in_modal_open) == "1")
    {
      this.openWindow();
    }    
  }

  ngOnChanges()
  {
    if (this.inputUserName != this.currentUserName)
    {//@Input的inputUserName變更要重新openWindow()，重新開啟暫存清單視窗，取得最新的UserName
      if (!this.currentUserName || this._localStorageService.getLocalStorageData(this.ls_key_take_in_modal_open) == "1")
      {
        this.openWindow();
      }

      this.currentUserName = this.inputUserName;
    }   
  }

  /**
   * 接收暫存清單彈跳視窗回傳是否更新主畫面訊號
   * @param event
   */
  @HostListener('window:message', ['$event'])
  onMessage(event: MessageEvent): void
  {
    if (event.data.data) {
      this.ProcessingPN(
        this.temp_save,
        this.boxInForm.controls["txt_pn"].value,
        this.boxInForm.controls["sel_location"].value,
        this.boxInForm.controls["sel_option"].value,
        this.boxInForm.controls["sel_style"].value,
        this.boxInForm.controls["txt_ssn"].value
      ) 
    }
  }


  @ViewChild("_txt_box_serial") boxing_serial: CounterPlusMinusComponent;
  @ViewChild("_txt_box_turtle_level") boxing_turtle_level: CounterPlusMinusComponent;
  @ViewChild("_txt_pn") txt_pn_element!: ElementRef<HTMLInputElement>;

  async GetInitData(): Promise<void> {    
    this.isLoading = true;
    try
    {
      this.temp_save = true;
      this.locations = await this._commonService.getBoxingLocations().toPromise();
      this.options = await this._commonService.getBoxingOptions().toPromise();
      this.style = await this._commonService.getBoxingStyle().toPromise();
    }
    catch (e) {
      this.Swal.fire({
        icon: "error",
        text: "初始資料載入失敗，請聯絡專案室.",
        showConfirmButton: false,
        allowOutsideClick: false,//點擊空白處關閉confirm
        allowEscapeKey: false//ESC關閉confirm
      })
    }
    this.isLoading = false;
  }

  public boxInForm: FormGroup = new FormGroup({
    sel_location: new FormControl(1, Validators.required)//庫房
    ,sel_option: new FormControl(1, Validators.required)//機台選項
    ,sel_style: new FormControl(1, Validators.required)//樣式選項
    ,txt_pn: new FormControl("", Validators.required)//P/N
    ,txt_ssn: new FormControl("", Validators.required)//P/N
    ,txt_model: new FormControl("")//Model
    ,txt_box_series: new FormControl("")//裝箱系列(箱名)
    ,txt_box_serial: new FormControl("")//箱號
    ,txt_box_qty: new FormControl("")//箱內數量
    ,chk_buffer_area: new FormControl(false)//暫存區
  })

  //P/N Blur & KeyUp事件
  ProcessingPN(saveTemp: boolean, partNumber: string, location: number, machineOtion: number, machineStyle: number, ssn: string)
  {
    this.alarmMsg = "";

    if (partNumber)
    {
      this.isLoading = true;

      this._boxInService.ProcessingPN(this.inputUserName, saveTemp, partNumber, location, machineOtion, machineStyle, ssn, this.getPnDataList()).toPromise()
        .then(res => {
          if (res.message != null)
          {
            this._soundPlayService.playSound(Enum_Sound.Error);
            this._swlService.showSwal("", res.message, "error");
            this.ResetMachineInfo();
            this.isLoading = false;
          }
          else {
            let data = res.content;

            if (this.temp_save)
            {//立即暫存
              this.saveTemp(data.partNumber_Model_Desc);
            }
            else
            {//不立即暫存
              //更新畫面input欄位
              this.pn_desc = data.partNumber_Model_Desc.desc;
              this.boxInForm.controls["sel_style"].setValue(data.partNumber_Model_Desc.boxing_Style_Id);
              this.boxInForm.controls["txt_model"].setValue(data.partNumber_Model_Desc.model);//Model
              this.boxInForm.controls["txt_box_series"].setValue(data.partNumber_Model_Desc.boxing_Series);//裝箱系列(箱名)
              this.boxing_serial.value = data.stocking_Info.box_Serial;//箱號
              this.boxing_serial.inputMinValue = data.stocking_Info.box_Serial;
              this.boxInForm.controls["txt_box_qty"].setValue(data.stocking_Info.qty);//箱內數量
              this.boxing_turtle_level.value = data.stocking_Info.turtle_Level;//烏龜車層數

              //Setting Boxing Info
              this.stocking_info_qty = data.stocking_Info.qty
              this.stocking_info_turtle_level = data.stocking_Info.turtle_Level;

              //Setting Boxing Info_Desc
              this._boxInService.GetStockingInfo(this.boxInForm.controls["txt_box_series"].value, this.boxInForm.controls["sel_location"].value, this.temp_save, this.getPnDataList()).toPromise().then(res => {
                this.refreshStockingInfoDesc(res.content);
              })

              this.pn_model_desc = data.partNumber_Model_Desc;
            }

            this.isLoading = false;
          }
        });
    }
  }

  //裝箱系列Blur & KeyUp事件
  ProcessingBoxSeries(boxSeries: string, location: number, saveTemp: boolean) {
    if (boxSeries) {
      //Setting Boxing Info_Desc
      this._boxInService.GetStockingInfo(boxSeries, location, saveTemp, this.getPnDataList()).toPromise()
        .then(res => {
          let data = res.content;
          this.boxing_serial.value = data.box_Serial;//箱號
          this.stocking_info_qty_desc = this.settingStockingInfoQtyDesc(data.box_Serial, data.qty, data.boxing_Db_Qty, data.boxing_Temp_Qty);//Setting Boxing Info_Desc
          this.boxing_serial.inputMinValue = data.box_Serial;
          this.boxInForm.controls["txt_box_qty"].setValue(data.qty);//箱內數量
          this.boxing_turtle_level.value = data.turtle_Level;//烏龜車層數
          this.stocking_info_qty = data.qty;
          this.stocking_info_turtle_level = data.turtle_Level;
        })
    }
  }

  //立即暫存Setting
  onSaveCheckboxChange(value: boolean) {
    this.temp_save = value;
  }
  
  //重新設定Input欄位為預設值      
  ResetMachineInfo(): void {
    this.pn_desc = "";
    //this.boxInForm.controls["sel_location"].setValue("1");
    //this.boxInForm.controls["sel_option"].setValue("1");
    //this.boxInForm.controls["sel_style"].setValue("1");
    this.boxInForm.controls["txt_pn"].setValue("");
    this.boxInForm.controls["txt_ssn"].setValue("");
    this.boxInForm.controls["txt_model"].setValue("");
    this.boxing_serial.inputMinValue = 1
    this.boxing_serial.value = 1
    this.boxInForm.controls["txt_box_qty"].setValue("");
    //Setting Boxing Info_Desc
    this.stocking_info_qty_desc = "";
    this.boxInForm.controls["txt_box_series"].setValue("");
    this.boxing_turtle_level.value = 1;
    this.pn_model_desc = undefined;
  }

  //Setting Boxing Info
  ResetBoxingStockingInfo(): void {    
    this.stocking_info_qty = -1;
    this.stocking_info_turtle_level = -1;
  }

  //設定箱內數量資訊
  settingStockingInfoQtyDesc(box_serial: string, totalQty: number, boxingDbQty: number, boxingTempQty: number): string {
    return `箱號${box_serial}，箱內數量總計：${totalQty} (資料庫已有數量：${boxingDbQty}，暫存筆數數量：${boxingTempQty})`
  }

  //從localStorage取得暫存資料
  getPnDataList(): PartNumber_Model_Desc[]
  {
    var json = this._localStorageService.getLocalStorageData(this.ls_key_list_data, JSON.stringify(this.array_pn_model_desc));
    if (json == "undefined") {
      return this.array_pn_model_desc;
    }

    return JSON.parse(json);
  }

  //清空localStorage
  resetPnDataList(): void {
    this.array_pn_model_desc = [];
    localStorage.setItem(this.ls_key_list_data, JSON.stringify(this.array_pn_model_desc));
  }

  //手動暫存資料，有參數為立即暫存
  saveTemp(pnData?: PartNumber_Model_Desc): void
  {
    if (
      (this.temp_save && !pnData!)/*立即暫存查無機台資訊*/ ||
      (!this.temp_save && !this.pn_model_desc)/*手動暫存查無機台資訊*/
    )
    {
      this._swlService.showSwal("", "請先查詢P/N取得機台資訊.", "error");
      return;
    }

    if (this.boxInForm.controls["txt_pn"].value)
    {
      //if (this.getPnDataList().length >= 100) {
      //  this._swlService.showSwal("", "暫存資料已達100筆，請先批次儲存.", "warning");
      //  return;
      //}

      if (!this.temp_save)
      {//沒有勾選立即暫存
        pnData = this.pn_model_desc;
        //重新取得可以手動更改的欄位
        pnData.ssn = this.boxInForm.controls["txt_ssn"].value;//SSN
        pnData.model = this.boxInForm.controls["txt_model"].value;//Model
        pnData.boxing_Series = this.boxInForm.controls["txt_box_series"].value;//裝箱系列
        pnData.boxing_Serial = this.boxing_serial.value;
        pnData.turtle_Level = this.boxing_turtle_level.value;
      }
      pnData.operator = this.inputUserName;//使用者


      /*由暫存清單資料取得是否要存放暫存區*/
      let isBufferArea = this.boxInForm.controls["chk_buffer_area"].value;//裝箱系列
      let tempData = this.getPnDataList();

      //設定當前資料的is_buffer_area
      pnData.is_Buffer_Area = isBufferArea;

      //設定暫存資料的is_buffer_area
      tempData.forEach(c => {
        if (c.boxing_Location_Id == pnData.boxing_Location_Id && c.boxing_Series == pnData.boxing_Series && c.boxing_Serial == pnData.boxing_Serial)
        {
          c.is_Buffer_Area = isBufferArea;
        }
      })

      this._localStorageService.setLocalStorageData(this.ls_key_list_data, tempData);

      //add data to array
      this.array_pn_model_desc = this.getPnDataList();
      this.array_pn_model_desc.push(pnData);
      this.array_pn_model_desc.forEach(function (item, index) {
        item.serial_No = index + 1;
      })
      localStorage.setItem(this.ls_key_list_data, JSON.stringify(this.array_pn_model_desc));

      //over 20 pcs
      this._boxInService.getStockingInfoByBoxSerial(pnData.boxing_Series, pnData.boxing_Location_Id, pnData.boxing_Serial, this.getPnDataList()).subscribe(
        res => {
          let data = res.content;
          if (data.qty == 20)
          {
            //play alarm audio
            this.alarmMsg = `箱號：${pnData.boxing_Serial}。箱內機台數已滿上限!`
            this._soundPlayService.playSound(Enum_Sound.Alarm)
          }
        },
        err => {

        }
      )

      //winform UnifyStackLevel()。新增暫存時，重新計算[第幾層]欄位
      if (this.currentPN != pnData.part_Number)
      {
        let newArray: PartNumber_Model_Desc[] = [];
        this.currentPN = pnData.part_Number;
        this.getPnDataList().forEach(c => {
          //同地點. 箱名. 箱號的暫存資料的[第幾層]欄位必須一樣
          if (c.boxing_Location_Id == pnData.boxing_Location_Id && c.boxing_Series == pnData.boxing_Series && c.boxing_Serial == pnData.boxing_Serial) {
            c.turtle_Level = pnData.turtle_Level;
          }
          newArray.push(c);
        });
        localStorage.setItem(this.ls_key_list_data, JSON.stringify(newArray));
      }

      if (this._localStorageService.getLocalStorageData(this.ls_key_take_in_modal_open) == "0")
      {
        this.openWindow();
      }

      this.postData();

      //reset input
      this.ResetMachineInfo();

      //Focus event
      this.txt_pn_element.nativeElement.focus();

      //Setting Boxing Info
      this.ResetBoxingStockingInfo();

    }
    else {
      this._swlService.showSwal("", "查無暫存資料.", "warning");
    }
  }

  //產出外箱貼紙，並批次上傳暫存資料(Promise Chain)
  batchSave() {
    this.isLoading = true;
    var data = this.getPnDataList();

    if (data.length <= 0) {
      this._swlService.showSwal("", "尚未刷入任一機台，無法批次儲存!", "warning");
      this.isLoading = false;
      return;
    }

    this._swlService.showSwalConfirm(
      "",
      "是否確認上傳資料並產出外箱貼紙?",
      "warning",
      () => {

        this._reportService.exportSticker(data).subscribe(
          resExportSticker => {

            this.getData(resExportSticker).then(
              (resExportSticker) => {
                console.log(`ExportSticker done. res：${resExportSticker}`);

                //debugger;
                console.log(`SaveBoxingInfos go. input parameter：${JSON.stringify(data)}`);
                this._boxInService.saveBoxingInfos(data).subscribe(
                  (resSaveBoxingInfos) => {
                    if (resSaveBoxingInfos.content > 0) {
                      this.ResetMachineInfo();
                      this.resetPnDataList();
                      this._swlService.showSwal("", `上傳完成，上傳資料筆數：${resSaveBoxingInfos.content}`, "warning");
                      this.postData(true);
                      console.log(`SaveBoxingInfos done. save success count：${resSaveBoxingInfos.content}`);

                      //debugger;
                      this.getData(resSaveBoxingInfos).then(
                        (resSaveBoxingInfos: IResultDto<number>) => {
                          console.log(`Process end. save success count：${resSaveBoxingInfos.content}`);
                          this.isLoading = false;
                        }
                      ).catch(
                        (err) => {
                          this.isLoading = false;
                        }
                      )
                    }
                  }
                )
              }
            ).catch(
              (err) => {
                this.isLoading = false;
              }
            )

          },
          err => {

            //批次上傳暫存資料
            this._soundPlayService.playSound(Enum_Sound.Error);
            this._swlService.showSwalConfirm(
              "",
              `無法產生Excel，是否依然上傳至資料庫?<br\>錯誤訊息：${err.message}`,
              "warning",
              //confirm callback function
              () => {
                this._boxInService.saveBoxingInfos(data).subscribe(res => {
                  if (res.content > 0) {
                    this.ResetMachineInfo();
                    this.resetPnDataList();
                    this._swlService.showSwal("", `上傳完成，上傳資料筆數：${res.content}`, "warning");
                    this.postData(true);
                    this.isLoading = false;
                  }
                })
              },
              () => {
                this.isLoading = false;
              }
            );

          }
        )
      },
      () => { this.isLoading = false; }
    );

  }

  /*
   *變更箱號事件
   *REF:https://www.angularfix.com/2022/01/angular-trigger-event-based-on-variable.html
   */
  OnChangeBoxingSerial(boxing_serial): void {

    if (this.stocking_info_qty != -1 && this.stocking_info_turtle_level != -1) {
      if (boxing_serial > this.boxing_serial.inputMinValue) {
        //使用未使用過的箱號，箱內數量跟烏龜車層數都設定為預設值：1
        this.boxInForm.controls["txt_box_qty"].setValue(0);
        this.boxing_turtle_level.value = 1;

        //Setting Boxing Info_Desc
        this.stocking_info_qty_desc = this.settingStockingInfoQtyDesc(boxing_serial, 0, 0, 0);
      }
      else if (boxing_serial == this.boxing_serial.inputMinValue) {
        //箱號調整為目前最大使用箱號，箱內數量跟烏龜車層數都設定為後端回傳的Stocking_Info
        this.boxInForm.controls["txt_box_qty"].setValue(this.stocking_info_qty);
        this.boxing_turtle_level.value = this.stocking_info_turtle_level;

        //Setting Boxing Info_Desc
        if (this.boxInForm.controls["txt_box_series"].value) {

          let box_series = this.boxInForm.controls["txt_box_series"].value;
          let location = this.boxInForm.controls["sel_location"].value;
          let saveTemp = this.temp_save;

          this._boxInService.GetStockingInfo(box_series, location, saveTemp, this.getPnDataList()).toPromise()
            .then(res => {
              this.refreshStockingInfoDesc(res.content);
            })
        }
      }
    }

  }

  //輸入P/N. 變更箱號. 箱內數量。重新取得儲位相關資訊
  refreshStockingInfoDesc(data:any): void {
    this.stocking_info_qty_desc = this.settingStockingInfoQtyDesc(data.box_Serial, data.qty, data.boxing_Db_Qty, data.boxing_Temp_Qty);
  }
 
  //開啟彈跳視窗
  openWindow(): void
  {
    if (this.isMultiOpen == false)
    {
      // 開啟目標視窗，如視窗未完成開啟前即執行 postMessage() 會傳送無效
      /*
       * popup parameters setting :https://javascript.info/popup-windows
       */
      this.previewWindow = window.open(`take_in_list/${this.inputUserName}`, "Take In Machine", "width=800,height=850");
      this._localStorageService.setLocalStorageData(this.ls_key_take_in_modal_open, 1)
    }
  }

  /**
   * 處理postMessage要傳給彈跳視窗的參數
   * @param isCloseWindow
   */
  postData(isCloseWindow?:boolean): void
  {
    if (!isCloseWindow)
    {
      isCloseWindow = false;
    }

    let tempData = this.getPnDataList();
    let passData: ITakeInPostMessageDto = {
      inputUserName: this.inputUserName,
      inputData: tempData[tempData.length - 1],
      isParentClose: isCloseWindow
    }

    if (this.previewWindow) {
      // 第二個參數 targetOrigin 為了示範使用故不指定，實務上應設定信任網域防止資訊外洩
      this.previewWindow.postMessage(passData, `${window.location.origin}/take_in_list`);
    }
  }

  /**
   * for API 結果傳遞呼叫(Promise Chain)
   * @param item 要傳遞給下一個API的參數
   * REF_1：https://stackblitz.com/edit/promise-chain-demo?file=index.ts
   * REF_2：https://www.casper.tw/javascript/2017/12/29/javascript-proimse/
   */
  getData = (item: any) => {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(item);
      }, 1000);
    });
  };

}
