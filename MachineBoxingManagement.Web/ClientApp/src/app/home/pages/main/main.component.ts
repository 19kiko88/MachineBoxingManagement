import { Component, OnInit } from '@angular/core';
import { CommonService } from '../../../core/http/common.service';
import { LocalStorageService } from '../../../core/services/local-storage.service';
import { SweetalertService } from '../../../core/services/sweetalert.service';

@Component({
  selector: 'app-main',
  templateUrl: './main.component.html',
  styleUrls: ['./main.component.css']
})
export class MainComponent implements OnInit {

  constructor(
    private _commonService: CommonService,
    private _localStorageService: LocalStorageService,
    private _swlService: SweetalertService
  )
  {

  }

  title = 'app';
  active = 1;
  userName = "";
  isMultiOpen: boolean = false;

  ngOnInit()
  {
    /*避免多視窗開啟MBM*/
    if (this._localStorageService.getLocalStorageData("isMbmOpened") != "1")
    {
      this._localStorageService.setLocalStorageData("isMbmOpened", 1);

      window.addEventListener("beforeunload", (event) =>
      {//監聽關閉(重整)主要視窗事件，isMbmOpened更新為0
        this._localStorageService.setLocalStorageData("isMbmOpened", 0);
      }, false);
    }
    else
    {
      this._swlService.showSwalNoButtonConfirm("", "MBM已經於其他分頁開啟.", "error");
      this.isMultiOpen = true;
    }

    this._localStorageService.removeLocalStorageData("isTakeInModalOpen");
    this._localStorageService.removeLocalStorageData("isTakeOutModalOpen");
    this._localStorageService.removeLocalStorageData("takeOutModalQueryCondition");

    //取得UserName
    this._commonService.getUserName().toPromise().then(res => { this.userName = res; })
  }

  onUserNameChange(name: string): void {
    this.userName = name;
  }
}
