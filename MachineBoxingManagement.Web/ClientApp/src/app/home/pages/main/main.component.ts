import { Component, OnInit, ViewChild } from '@angular/core';
import { SweetalertService } from '../../../core/services/sweetalert.service';
import * as uuid from 'uuid';
import { MultiselectDropdownComponent } from '../../../shared/multiselect-dropdown/multiselect-dropdown.component';
import { Router } from '@angular/router';
import jwtDecode from 'jwt-decode';
import { LocalStorageKey } from '../../../shared/models/localstorage-model';
import * as ls from "local-storage";
import { PartNumber_Model_Desc } from '../../../shared/models/dto/response/box-in';
import { boxOutQueryCondition } from '../../../shared/models/dto/request/box-out-query-condition';


@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent implements OnInit {

  @ViewChild("_multiselect_dropdown") multiselectDropdown: MultiselectDropdownComponent;

  constructor(
    private _swlService: SweetalertService,
    private _router: Router
  )
  {

  }

  title = 'app';
  active = 1;
  userName = "";
  isMultiOpen: boolean = false;
  userList = [];
  isLoading: boolean = false;
  initUUID: uuid = uuid.v4();//main.ts初始UUID


  async ngOnInit()
  {
    this.isLoading = true;

    //LocalStorage 資料設定預設值
    ls.set<uuid>(LocalStorageKey.uuid, this.initUUID);//uuid
    ls.set<number>(LocalStorageKey.isTakeInModalOpen, 0);//裝箱維護modal是否開啟。    
    ls.set<number>(LocalStorageKey.isTakeOutModalOpen, 0);//取出維護modal是否開啟。
    ls.set<string>(LocalStorageKey.operators, "");//operators => MBM操作者
    ls.set<boxOutQueryCondition>(LocalStorageKey.takeOutModalQueryCondition, null);//takeOutModalQueryCondition => 取出維護查詢條件
    ls.set<PartNumber_Model_Desc[]>(LocalStorageKey.bufferAreas, []);//bufferAreas => 存放暫存區機台，有資料時不清除
    //tempListData => 裝箱維護暫存機台List，有資料時不清除
    const ls_tempListData = ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.tempListData);
    if (!ls_tempListData) ls.set<PartNumber_Model_Desc[]>(LocalStorageKey.tempListData, []);
    //themeType => 主題配色，有資料時不清除
    const ls_themeType = ls.get<number>(LocalStorageKey.themeType);
    if (!ls_themeType) ls.set<number>(LocalStorageKey.themeType, 1);
    //myFavorite => 取出維護modal的暫存List，有資料時不清除
    const ls_myFavorite = ls.get<PartNumber_Model_Desc[]>(LocalStorageKey.myFavorite);
    if (!ls_myFavorite) ls.set<PartNumber_Model_Desc[]>(LocalStorageKey.myFavorite, []);


    const initJwt = ls.get<string>(LocalStorageKey.jwt);//main.ts初始JWT
    var intervalId = window.setInterval(() => {
      const currentUUID = ls.get<uuid>(LocalStorageKey.uuid)//取得最新UUID
      const currentJWT = ls.get<string>(LocalStorageKey.jwt);//取得最新JWT
      let jwtValidate = false

      if (currentJWT)
      {
        const payload: any = jwtDecode(currentJWT);
        const exp = new Date(Number(payload.exp) * 1000);

        if (new Date() < exp)
        {//檢核JWT是否過期
          jwtValidate = true;
        }
      }

      if (jwtValidate == false)
      {
        clearInterval(intervalId);//結束interval
        ls.set<number>(LocalStorageKey.isTakeInModalOpen, -1);//強制關閉裝箱維護modal
        ls.set<number>(LocalStorageKey.isTakeOutModalOpen, -1);//強制關閉取出維護modal

        this._swlService.showSwalConfirm("", "部門驗證授權已經過期，點選[是]取得授權.", "error", () => {
          ls.set<string>(LocalStorageKey.jwt, "");
          this._router.navigateByUrl('\auth_check');//轉址到授權頁面
        }, null, null, null, false);
      }

      if (initJwt == currentJWT && this.initUUID != currentUUID)
      {
        /*重複開啟檢核邏輯
        *1.進入main.ts會先取得一組初始UUID存放於initUUID & localStorage["uuid"]
        *2.interval取得localStorage["uuid"]後，會檢查initUUID & localStorage["uuid"]的值是否相同
        *   相同=>未重複開啟
        *   不同=>於其他分頁重複開啟main，導致localStorage["uuid"]被更新
        *3.避免取得部門授權流程中(由authentication-check=>main)，currentUUID已經於initUUID: uuid = uuid.v4();被換掉，但interval非同步還是在檢核this.initUUID != currentUUID
        *  導致this.initUUID(已經是新的UUID) != currentUUID(還是舊的UUID)而出現[MBM已經於其他分頁開啟,<br\>請於最新開啟分頁進行操作.]，
        *  故加上this.initJwt == currentJWT條件：
        *  1.this.initJwt == currentJWT表示沒有進行取得部門授權流程
        *  2.this.initJwt != currentJWT表示進行取得部門授權流程中，不進行this.initUUID != currentUUID檢核
        *  部門授權流程完成後，this.initJwt 就會等於 currentJWT，開始進行this.initUUID != currentUUID檢核
        * */
        this.isMultiOpen = true;

        ls.set<number>(LocalStorageKey.isTakeInModalOpen, -1);//強制關閉裝箱維護modal
        ls.set<number>(LocalStorageKey.isTakeOutModalOpen, -1);//強制關閉取出維護modal

        this._swlService.showSwalNoButtonConfirm("", "MBM已經於其他分頁開啟,<br\>請於最新開啟分頁進行操作.", "error");
        clearInterval(intervalId);//結束interval
      }
    }, 1000)

    const jwtData: any = jwtDecode(initJwt);
    const deptMembers = JSON.parse(jwtData.DeptMembers);
    let tempArray = [];
    for (var i = 0; i < deptMembers.length; i++)
    {
      tempArray.push({ item_id: deptMembers[i].item_id, item_text: deptMembers[i].item_text })
    }
    this.userList = tempArray;//for multiselect-dropdown.component.ts的OnChange事件塞值
    this.isLoading = false;
  }

  onUserNameChange(operators: string): void
  {
    const operatorList: any = JSON.parse(operators);
    let strOperators: string = "";

    if (operatorList.length > 0)
    {
      for (var i = 0; i < operatorList.length; i++)
      {
        let splitIdx = operatorList[i].item_text.indexOf("(");
        strOperators += `${operatorList[i].item_text.substring(splitIdx + 1, operatorList[i].item_text.length - 1)},`;
      }
    }

    if (!strOperators)
    {
      this.multiselectDropdown.selectedItems = [];
    }

    this.userName = strOperators;

    //操作者清空後，main區塊會被拿掉，無法觸發box-in & box-out的OnChange。故用localStorage紀錄操作者，沒有操作者就通知modal關閉。
    ls.set<string>(LocalStorageKey.operators, strOperators);
  }

  setOperators(operatorList: any)
  {
    if (!operatorList)
    {
      operatorList = {};
    }
    this.onUserNameChange(JSON.stringify(operatorList));
  }
}
