import { NgModule, Testability } from '@angular/core';
import { Routes, RouterModule } from '@angular/router'
import { CommonModule } from '@angular/common';
import { BoxInComponent } from './home/pages/box-in/box-in.component';
import { BoxOutComponent } from './home/pages/box-out/box-out.component';
import { MainComponent } from './home/pages/main/main.component';
import { TempListModalComponent } from './home/pages/take-in-modal/take-in-modal.component';
import { TakeOutModalComponent } from './home/pages/take-out-modal/take-out-modal.component';
import { AuthorizeCheckComponent } from './home/pages/authorize-check/authorize-check.component';
import { AuthGuard } from './auth.guard';

const routes: Routes = [
  { path: '', component: AuthorizeCheckComponent },//預設路由，檢核是否有部門權限併發JWT
  { path: 'auth_check', component: AuthorizeCheckComponent },
  { path: 'main', component: MainComponent, canActivate: [AuthGuard] },//AuthGuard檢核JWT是否有效，避免user直接透過url進入頁面
  { path: 'take_in_list/:user_name/:uuid', component: TempListModalComponent, canActivate: [AuthGuard] },
  { path: 'take_out_list/:user_name/:uuid', component: TakeOutModalComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }
