import { Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import jwtDecode from 'jwt-decode';
import { Observable } from 'rxjs';
import { SweetalertService } from './core/services/sweetalert.service';
import { LocalStorageKey } from './shared/models/localstorage-model';

@Injectable({
  providedIn: 'root'
})

  /**
   * [路由守衛]
   * 進入URL前，要先檢核是否有核JWT & JWT是否過期
   */
export class AuthGuard implements CanActivate {

  constructor(
    private _swlService: SweetalertService,
    private router: Router) { }

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    const jwt = localStorage.getItem(LocalStorageKey.jwt);
    let errMsg: string = "";

    if (jwt && jwt != "undefined")
    {
      const payload:any = jwtDecode(jwt);
      const exp = new Date(Number(payload.exp) * 1000);

      if (new Date() > exp)
      {
        errMsg = '部門驗證授權已經過期，點選[是]取得授權.'
      }
    }
    else
    {
      errMsg = '尚未取得部門驗證授權，點選[是]取得授權.'
    }

    if (errMsg)
    {
      alert(errMsg);
      console.log(errMsg)
      return this.router.createUrlTree(['/auth_check']);
    }
    else
    {
      return true;
    }

  }
  
}
