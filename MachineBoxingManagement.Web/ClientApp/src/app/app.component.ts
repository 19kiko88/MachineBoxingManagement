import { Component, OnChanges, OnInit } from '@angular/core';
import { CommonService } from './core/http/common.service';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent implements OnInit {

  constructor(private _commonService: CommonService) {

  }

  title = 'app';
  active = 1;
  userName = "";

  ngOnInit() {
    //取得UserName
    this._commonService.getUserName().toPromise().then(res => { this.userName = res; })
  }

  onUserNameChange(name: string): void {
    this.userName = name;
  }
}
