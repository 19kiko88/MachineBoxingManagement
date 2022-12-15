import { Component, OnInit } from '@angular/core';
import { environment } from '../environments/environment';
import { LocalStorageKey } from './shared/models/localstorage-model';
import * as ls from "local-storage";


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {

  constructor() { }

  title = 'app';
  active = 1;
  userName = "";
  
  async ngOnInit()
  {

    let theme_type = ls.get<number>(LocalStorageKey.themeType);
    document.documentElement.style.setProperty("--body", theme_type == 0 ? "" : "#343a40");
    document.documentElement.style.setProperty("--logo_path", `url(${environment.webSite}/assets/logo.png) no-repeat`);
    document.documentElement.style.setProperty("--label_font", theme_type == 0 ? "" : "#ffffff");
    document.documentElement.style.setProperty("--alert_msg", theme_type == 0 ? "" : "#ffffff");
    document.documentElement.style.setProperty("--nav_li", "#ffffff");

  }

  onUserNameChange(name: string): void {
    this.userName = name;
  }
}
