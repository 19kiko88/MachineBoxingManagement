import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { NavMenuComponent } from './nav-menu/nav-menu.component';

import { CounterPlusMinusComponent } from './shared/counter-plus-minus/counter-plus-minus.component';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from './shared/modules/shared.module'
import { BoxInComponent } from './home/pages/box-in/box-in.component';
import { BoxOutComponent } from './home/pages/box-out/box-out.component';
import { CoreModule } from './core/core.module';
import { LoaderComponent } from './shared/loader/loader.component';
import { AppRoutingModule } from './app-routing.module';
import { MainComponent } from './home/pages/main/main.component';
import { TempListModalComponent } from './home/pages/take-in-modal/take-in-modal.component';
import { TakeOutModalComponent } from './home/pages/take-out-modal/take-out-modal.component';
import { MatIconModule } from '@angular/material/icon';
import { FavoriteButtonModule } from './shared/favorite-button/favorite-button.module';
import { TempDataModalComponent } from './home/pages/temp-data-modal/temp-data-modal.component';
import { NgbdDatepickerRangePopup } from './shared/datepicker-range/datepicker-range.component';
import { NgbdDatepickerPopup } from './shared/datepicker/datepicker.component';

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    BoxInComponent,
    CounterPlusMinusComponent,
    BoxOutComponent,
    LoaderComponent,
    MainComponent,
    TempListModalComponent,
    TakeOutModalComponent,
    TempDataModalComponent,
    NgbdDatepickerRangePopup,
    NgbdDatepickerPopup
  ],
  imports: [
    BrowserModule/*.withServerTransition({ appId: 'ng-cli-universal' })*/,
    CoreModule,
    HttpClientModule,
    FormsModule,
    /*Routing改由app-routing.module.ts控制*/
    //RouterModule.forRoot([
    //  { path: '', component: MainComponent, pathMatch: 'full' },
    //], { relativeLinkResolution: 'legacy' }),    
    ReactiveFormsModule,
    BrowserAnimationsModule,
    SharedModule,
    AppRoutingModule,
    MatIconModule,
    FavoriteButtonModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
