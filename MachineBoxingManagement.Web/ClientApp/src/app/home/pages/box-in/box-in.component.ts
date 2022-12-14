import { Component, Input, Output, OnInit, ViewChild, AfterViewInit, ElementRef, HostListener, AfterViewChecked, OnChanges, EventEmitter } from '@angular/core';
import { async } from '@angular/core/testing';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { DropDowList } from '../../../shared/models/dto/response/drop-down-list';
import { PartNumber_Model_Desc } from '../../../shared/models/dto/response/box-in';
import { CounterPlusMinusComponent } from './../../../shared/counter-plus-minus/counter-plus-minus.component'
import { BoxInService } from '../../../core/http/box-in.service';
import { CommonService } from './../../../core/http/common.service';
import { ReportService } from '../../../core/http/report.service';
import { SweetalertService } from './../../../core/services/sweetalert.service';
import Swal from 'sweetalert2';
import { SoundPlayService } from '../../../core/services/sound-play.service';
import { Enum_Sound } from '../../../shared/models/enum/sound';
import { IResultDto } from '../../../shared/models/dto/result-dto';
import { LocalStorageKey } from '../../../shared/models/localstorage-model';
import * as ls from "local-storage";
import { IPostMessage } from '../../../shared/models/post-message'
import { ModalOptionDetailComponent } from '../box-out/modal-option-detail/modal-option-detail.component';

@Component({
  selector: 'app-box-in',
  templateUrl: './box-in.component.html',
  styleUrls: ['./box-in.component.css']
})

export class BoxInComponent implements OnInit, OnChanges {
  @Input() activeNo: string;
  @Input() inputUserName: string;
  @Input() inputUUID;
  @Output() outputUserName: EventEmitter<any> = new EventEmitter<any>();
  
  locations: DropDowList[] = [] ;
  options: any[] = [];
  style: any[] = [];
  temp_save: boolean = false;
  pn_desc: string;
  stocking_info_qty: number = -1;
  stocking_info_turtle_level: number = -1;
  stocking_info_qty_desc: string;
  isLoading: boolean = false;
  currentPN: string = "";
  Swal = Swal;
  alarmMsg: string = "";
  previewWindow: Window; // ??????????????? window ??????
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
    private _modalService: NgbModal
  )
  { }

  ngOnInit()
  {
    ls.set<number>(LocalStorageKey.isTakeOutModalOpen, -1);//?????????????????????????????????????????????modal??????

    this.GetInitData();

    /*??????????????????????????????????????????*/
    window.addEventListener("beforeunload", (event) => {
      ls.set<number>(LocalStorageKey.isTakeInModalOpen, -1);
    }, false);
  }

  //ngOnChanges??????@Input?????????????????????
  ngOnChanges()
  {//@Input???inputUserName???????????????openWindow()???????????????????????????????????????????????????UserName
    //???????????????
    if (this.inputUserName != this.currentUserName)
    {
      if (this.currentUserName && ls.get<number>(LocalStorageKey.isTakeInModalOpen) == 1)
      {
          this.openWindow();
        }
      this.currentUserName = this.inputUserName;
    }
  }

  /**
   * ???????????????????????????????????????????????????????????????
   * @param event
   */
  @HostListener('window:message', ['$event'])
  onMessage(event: MessageEvent<IPostMessage>): void
  {
    if (event.data.refreshMain)
    {
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

  GetInitData() {    
    this.isLoading = true;

    try
    {
      this.temp_save = true;
      this._commonService.getBoxingLocations().subscribe(res => { this.locations = res });
      this._commonService.getBoxingOptions().subscribe(res => { this.options = res });
      this._commonService.getBoxingStyle().subscribe(res => { this.style = res });
    }
    catch (e) {
      this.Swal.fire({
        icon: "error",
        text: "?????????????????????????????????????????????.",
        showConfirmButton: false,
        allowOutsideClick: false,//?????????????????????confirm
        allowEscapeKey: false//ESC??????confirm
      })
    }
    finally
    {
      this.isLoading = false;
    }
  }

  public boxInForm: FormGroup = new FormGroup({
    sel_location: new FormControl(1, Validators.required)//??????
    ,sel_option: new FormControl(1, Validators.required)//????????????
    ,sel_style: new FormControl(1, Validators.required)//????????????
    ,txt_pn: new FormControl("", Validators.required)//P/N
    ,txt_ssn: new FormControl("", Validators.required)//P/N
    ,txt_model: new FormControl("")//Model
    ,txt_box_series: new FormControl("")//????????????(??????)
    ,txt_box_serial: new FormControl("")//??????
    ,txt_box_qty: new FormControl("")//????????????
    ,chk_buffer_area: new FormControl(false)//?????????
  })

  //P/N Blur & KeyUp??????
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
            {//????????????
              this.saveTemp(data.partNumber_Model_Desc);
            }
            else
            {//???????????????
              //????????????input??????
              this.pn_desc = data.partNumber_Model_Desc.desc;
              this.boxInForm.controls["sel_style"].setValue(data.partNumber_Model_Desc.boxing_Style_Id);
              this.boxInForm.controls["txt_model"].setValue(data.partNumber_Model_Desc.model);//Model
              this.boxInForm.controls["txt_box_series"].setValue(data.partNumber_Model_Desc.boxing_Series);//????????????(??????)
              this.boxing_serial.value = data.stocking_Info.box_Serial;//??????
              this.boxing_serial.inputMinValue = data.stocking_Info.box_Serial;
              this.boxInForm.controls["txt_box_qty"].setValue(data.stocking_Info.qty);//????????????
              this.boxing_turtle_level.value = data.stocking_Info.turtle_Level;//???????????????

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
        })
        .catch(ex => {
          this._swlService.showSwal("", `P/N??????????????????????????????CAE Team??????????????????<br\>${ex}`, "error");
          this.isLoading = false;
        })
    }
  }

  //????????????Blur & KeyUp??????
  ProcessingBoxSeries(boxSeries: string, location: number, saveTemp: boolean) {
    if (boxSeries) {
      //Setting Boxing Info_Desc
      this._boxInService.GetStockingInfo(boxSeries, location, saveTemp, this.getPnDataList()).toPromise()
        .then(res => {
          let data = res.content;
          this.boxing_serial.value = data.box_Serial;//??????
          this.stocking_info_qty_desc = this.settingStockingInfoQtyDesc(data.box_Serial, data.qty, data.boxing_Db_Qty, data.boxing_Temp_Qty);//Setting Boxing Info_Desc
          this.boxing_serial.inputMinValue = data.box_Serial;
          this.boxInForm.controls["txt_box_qty"].setValue(data.qty);//????????????
          this.boxing_turtle_level.value = data.turtle_Level;//???????????????
          this.stocking_info_qty = data.qty;
          this.stocking_info_turtle_level = data.turtle_Level;
        })
    }
  }

  //????????????Setting
  onSaveCheckboxChange(value: boolean) {
    this.temp_save = value;
  }
  
  //????????????Input??????????????????      
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

  //????????????????????????
  settingStockingInfoQtyDesc(box_serial: string, totalQty: number, boxingDbQty: number, boxingTempQty: number): string {
    return `??????${box_serial}????????????????????????${totalQty} (????????????????????????${boxingDbQty}????????????????????????${boxingTempQty})`
  }

  //???localStorage??????????????????
  getPnDataList(): PartNumber_Model_Desc[]
  {
    const tempListData = ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.tempListData);
    return tempListData;
  }

  //??????localStorage
  resetPnDataList(): void {
    this.array_pn_model_desc = [];
    localStorage.setItem(LocalStorageKey.tempListData, JSON.stringify(this.array_pn_model_desc));
  }

  //?????????????????????????????????????????????
  saveTemp(pnData?: PartNumber_Model_Desc): void
  {
    if (
      (this.temp_save && !pnData!)/*??????????????????????????????*/ ||
      (!this.temp_save && !this.pn_model_desc)/*??????????????????????????????*/
    )
    {
      this._swlService.showSwal("", "????????????P/N??????????????????.", "warning");
      return;
    }

    if (this.boxInForm.controls["txt_pn"].value)
    {
      if (!this.temp_save)
      {//????????????????????????
        pnData = this.pn_model_desc;
        //???????????????????????????????????????
        pnData.ssn = this.boxInForm.controls["txt_ssn"].value;//SSN
        pnData.model = this.boxInForm.controls["txt_model"].value;//Model
        pnData.boxing_Series = this.boxInForm.controls["txt_box_series"].value;//????????????
        pnData.boxing_Serial = this.boxing_serial.value;
        pnData.turtle_Level = this.boxing_turtle_level.value;
      }
      pnData.operator = this.inputUserName;//?????????


      /*???????????????????????????????????????????????????*/
      let isBufferArea = this.boxInForm.controls["chk_buffer_area"].value;//????????????
      let tempData = this.getPnDataList();

      //?????????????????????is_buffer_area
      pnData.is_Buffer_Area = isBufferArea;

      //?????????????????????is_buffer_area
      tempData.forEach(c => {
        if (c.boxing_Location_Id == pnData.boxing_Location_Id && c.boxing_Series == pnData.boxing_Series && c.boxing_Serial == pnData.boxing_Serial)
        {
          c.is_Buffer_Area = isBufferArea;
        }
      })

      ls.set<PartNumber_Model_Desc[]>(LocalStorageKey.tempListData, tempData);

      //add data to array
      this.array_pn_model_desc = this.getPnDataList();
      this.array_pn_model_desc.push(pnData);
      this.array_pn_model_desc.forEach(function (item, index) {
        item.serial_No = index + 1;
      })
      localStorage.setItem(LocalStorageKey.tempListData, JSON.stringify(this.array_pn_model_desc));

      //over 20 pcs
      this._boxInService.getStockingInfoByBoxSerial(pnData.boxing_Series, pnData.boxing_Location_Id, pnData.boxing_Serial, this.getPnDataList()).subscribe(
        res => {
          let data = res.content;
          if (data.qty == 20)
          {
            //play alarm audio
            this.alarmMsg = `?????????${pnData.boxing_Serial}??????????????????????????????!`
            this._soundPlayService.playSound(Enum_Sound.Alarm)
          }
        },
        err => {

        }
      )

      //winform UnifyStackLevel()?????????????????????????????????[?????????]??????
      if (this.currentPN != pnData.part_Number)
      {
        let newArray: PartNumber_Model_Desc[] = [];
        this.currentPN = pnData.part_Number;
        this.getPnDataList().forEach(c => {
          //?????????. ??????. ????????????????????????[?????????]??????????????????
          if (c.boxing_Location_Id == pnData.boxing_Location_Id && c.boxing_Series == pnData.boxing_Series && c.boxing_Serial == pnData.boxing_Serial) {
            c.turtle_Level = pnData.turtle_Level;
          }
          newArray.push(c);
        });
        localStorage.setItem(LocalStorageKey.tempListData, JSON.stringify(newArray));
      }

      if (ls.get<number>(LocalStorageKey.isTakeInModalOpen) == 0)
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
      this._swlService.showSwal("", "??????????????????.", "warning");
    }
  }

  //????????????????????????????????????????????????(Promise Chain)
  batchSave() {
    this.isLoading = true;
    var data = this.getPnDataList();

    if (data.length <= 0) {
      this._swlService.showSwal("", "???????????????????????????????????????!", "warning");
      this.isLoading = false;
      return;
    }

    this._swlService.showSwalConfirm(
      "",
      "??????????????????????????????????????????????",
      "warning",
      () => {
        this._reportService.exportSticker(data).subscribe(
          resExportSticker => {
            this.getData(resExportSticker).then(
              (resExportSticker) => {
                console.log(`ExportSticker done. res???${resExportSticker}`);

                //debugger;
                console.log(`SaveBoxingInfos go. input parameter???${JSON.stringify(data)}`);
                this._boxInService.saveBoxingInfos(data).subscribe(
                  (resSaveBoxingInfos) => {
                    if (resSaveBoxingInfos.content > 0) {
                      this.ResetMachineInfo();
                      this.resetPnDataList();
                      this._swlService.showSwal("", `????????????????????????????????????${resSaveBoxingInfos.content}`, "warning");
                      ls.set<number>(LocalStorageKey.isTakeInModalOpen, -1);
                      this.postData();
                      console.log(`SaveBoxingInfos done. save success count???${resSaveBoxingInfos.content}`);

                      //debugger;
                      this.getData(resSaveBoxingInfos).then(
                        (resSaveBoxingInfos: IResultDto<number>) => {
                          console.log(`Process end. save success count???${resSaveBoxingInfos.content}`);                          
                        }
                      ).finally(() => {
                          this.isLoading = false;
                          this.outputUserName.emit("");
                        }
                      )
                    }
                  }
                )
              }
            ).finally(() => {
                this.isLoading = false;
                this.outputUserName.emit("");
              }
            )
          },
          err => {

            //????????????????????????
            this._soundPlayService.playSound(Enum_Sound.Error);
            this._swlService.showSwalConfirm(
              "",
              `????????????Excel??????????????????????????????????<br\>???????????????${err.message}`,
              "warning",
              //confirm callback function
              () => {
                this._boxInService.saveBoxingInfos(data).subscribe(res => {
                  if (res.content > 0) {
                    this.ResetMachineInfo();
                    this.resetPnDataList();
                    this._swlService.showSwal("", `????????????????????????????????????${res.content}`, "warning");
                    ls.set<number>(LocalStorageKey.isTakeInModalOpen, -1);
                    this.postData();
                    this.isLoading = false;
                    this.outputUserName.emit("");                  }
                },
                (err) => {
                  //error
                  this.isLoading = false;
                  this._swlService.showSwal("", "????????????????????????CAE Team.", "error");
                })
              },
              () => {
                this.isLoading = false;
              }
            );

          }
        )
      },
      () => {
        this.isLoading = false;
      }
    );

  }

  /*
   *??????????????????
   *REF:https://www.angularfix.com/2022/01/angular-trigger-event-based-on-variable.html
   */
  OnChangeBoxingSerial(boxing_serial): void {

    if (this.stocking_info_qty != -1 && this.stocking_info_turtle_level != -1) {
      if (boxing_serial > this.boxing_serial.inputMinValue) {
        //????????????????????????????????????????????????????????????????????????????????????1
        this.boxInForm.controls["txt_box_qty"].setValue(0);
        this.boxing_turtle_level.value = 1;

        //Setting Boxing Info_Desc
        this.stocking_info_qty_desc = this.settingStockingInfoQtyDesc(boxing_serial, 0, 0, 0);
      }
      else if (boxing_serial == this.boxing_serial.inputMinValue) {
        //???????????????????????????????????????????????????????????????????????????????????????????????????Stocking_Info
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

  //??????P/N. ????????????. ?????????????????????????????????????????????
  refreshStockingInfoDesc(data:any): void {
    this.stocking_info_qty_desc = this.settingStockingInfoQtyDesc(data.box_Serial, data.qty, data.boxing_Db_Qty, data.boxing_Temp_Qty);
  }
 
  //??????????????????
  openWindow(): void
  {
    if (ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.tempListData).length > 0)
    {
      /*
       * popup parameters setting :https://javascript.info/popup-windows
       */
      this.previewWindow = window.open(`take_in_list/${this.inputUserName}/${this.inputUUID}`, "Take In Machine", "width=800,height=850");
      ls.set<number>(LocalStorageKey.isTakeInModalOpen, 1);
    }
    else
    {
      this._swlService.showSwal("", "??????????????????.", "warning");
    }
  }

  /**
   * ??????postMessage??????????????????????????????
   */
  postData(): void
  {
    const tempData = this.getPnDataList();

    if (this.previewWindow)
    {
      const passData: IPostMessage = { inputUserName: this.inputUserName, boxinInputData: tempData[tempData.length - 1] }
      this.previewWindow.postMessage(passData, `${window.location.origin}/take_in_list`);// ??????????????? targetOrigin ?????????????????????????????????????????????????????????????????????????????????
    }
  }

  /**
   * for API ??????????????????(Promise Chain)
   * @param item ?????????????????????API?????????
   * REF_1???https://stackblitz.com/edit/promise-chain-demo?file=index.ts
   * REF_2???https://www.casper.tw/javascript/2017/12/29/javascript-proimse/
   */
  getData = (item: any) => {
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        resolve(item);
      }, 1000);
    });
  };

  //????????????????????????
  async openOptionDetail() {
    const modalRef = this._modalService.open(ModalOptionDetailComponent);
    let textOptions: string = '';

    this._commonService.getBoxingOptions().subscribe(res => {
      if (res.length > 0)
      {
        res.forEach(c => {
          textOptions += `${c.name += c.remark}\r\n`;
        });
        modalRef.componentInstance.options = textOptions;
      }
    })
  }
}
