import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { SweetalertService } from '../../../core/services/sweetalert.service';
import { LocalStorageKey } from '../../../shared/models/localstorage-model';
import * as ls from "local-storage";

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent implements OnInit {

  constructor(
    private _swlService: SweetalertService
  ) {}

  //綁定FormGroup input
  settingsForm: FormGroup = new FormGroup({
    selThemeType: new FormControl(1, Validators.required)//庫房
  })

  ngOnInit(): void
  {
    this.settingsForm.controls["selThemeType"].setValue(ls.get<number>(LocalStorageKey.themeType));
  }

  //儲存設定
  saveSettings()
  {



    this._swlService.showSwalConfirm(
      "",
      "是否儲存設定?<br\>點選[是]後，MBM會立即重整畫面，並套用新設定.",
      "warning",
      () => {
        let themeType: number = this.settingsForm.controls["selThemeType"].value;//string to number
        ls.set<number>(LocalStorageKey.themeType, themeType);
        window.location.reload();
      },
      () => {
        return;
      }
    );
  }


}
