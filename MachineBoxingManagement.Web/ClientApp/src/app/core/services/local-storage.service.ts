import { Inject, inject, Injectable } from '@angular/core';


@Injectable({
  providedIn: 'root'
})

export class LocalStorageService {

  constructor() { }

  //資料存放至localStorage
  setLocalStorageData<T>(localStorageKey:string, data: T): void {
    localStorage.setItem(localStorageKey, JSON.stringify(data));
  }

  //從localStorage取得資料
  getLocalStorageData(localStorageKey: string, defaultvalue?: any): string
  {
    if (!localStorage.getItem(localStorageKey) && !defaultvalue)
    {
      localStorage.setItem(localStorageKey, defaultvalue);
    }

    return localStorage.getItem(localStorageKey);
  }

  //清除localStorage
  removeLocalStorageData(localStorageKey: string,): void {
    localStorage.removeItem(localStorageKey);
  }
}
