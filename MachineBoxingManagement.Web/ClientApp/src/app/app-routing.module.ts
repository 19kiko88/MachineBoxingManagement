import { NgModule, Testability } from '@angular/core';
import { Routes, RouterModule } from '@angular/router'
import { CommonModule } from '@angular/common';
import { BoxInComponent } from './home/pages/box-in/box-in.component';
import { BoxOutComponent } from './home/pages/box-out/box-out.component';
import { MainComponent } from './home/pages/main/main.component';
import { TempListModalComponent } from './home/pages/take-in-modal/take-in-modal.component';
import { TakeOutModalComponent } from './home/pages/take-out-modal/take-out-modal.component';

const routes: Routes = [
  {path: '', component: MainComponent},
  { path: 'take_in_list/:user_name', component: TempListModalComponent },
  { path: 'take_out_list/:user_name', component: TakeOutModalComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})

export class AppRoutingModule { }
