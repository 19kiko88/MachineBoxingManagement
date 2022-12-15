import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import jwtDecode from 'jwt-decode';
import { CommonService } from '../../../core/http/common.service';
import { JwtService } from '../../../core/http/jwt.service';
import { LocalStorageKey } from '../../../shared/models/localstorage-model';
import * as ls from "local-storage";

@Component({
  selector: 'app-authorize-check',
  templateUrl: './authorize-check.component.html',
  styleUrls: ['./authorize-check.component.css']
})

/*
 *[預設首頁]
 *檢核是否有部門權限，有的話就發JWT，沒有的話不進行後續動作。 
*/
export class AuthorizeCheckComponent implements OnInit {

  msg: string = "部門權限檢核中...";

  constructor(
    private _router: Router,
    private _jwtService: JwtService,
    private _commonService: CommonService
  ) { }

  async ngOnInit()
  {
    let DeptCheck = await this._commonService.DeptCheck().toPromise();//部門檢核
    const jwt = ls.get<string>(LocalStorageKey.jwt);

    if (!DeptCheck.message)
    {
      if (jwt)
      {//已經有JWT
        const payload: any = jwtDecode(jwt);
        const exp = new Date(Number(payload.exp) * 1000);
        if (new Date() > exp)
        {
          ls.set<string>(LocalStorageKey.jwt, "");
          alert('部門驗證授權已經過期，請重新取得授權.');
          this._router.navigateByUrl('/auth_check');//轉址到主頁面
        }
        else
        {
          this._router.navigateByUrl('\main');//轉址到主頁面
        }
      }
      else
      {//沒有JWT
        this._jwtService.getJwt().subscribe(res =>
        {
          ls.set<string>(LocalStorageKey.jwt, res.content);//取得JWT
          this._router.navigateByUrl('\main');//轉址到主頁面
        })        
      }
    }
    else
    {
      ls.set<string>(LocalStorageKey.jwt, "");
      this.msg = DeptCheck.message;
    }
  }

}
