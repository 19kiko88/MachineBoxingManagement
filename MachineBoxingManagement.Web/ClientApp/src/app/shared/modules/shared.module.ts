import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { NgbModule } from "@ng-bootstrap/ng-bootstrap";

import { MatTableModule } from '@angular/material/table';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';

//angular material modules
const materialModules = [
  MatTableModule,
  MatInputModule,
  MatCheckboxModule,
  MatFormFieldModule
]

//ng-bootstrap modules
const ngBootstrapModules = [
  NgbModule
]

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    ...ngBootstrapModules,
    ...materialModules
  ],
  exports: [
    ...ngBootstrapModules,
    ...materialModules
  ]
})

export class SharedModule { }
