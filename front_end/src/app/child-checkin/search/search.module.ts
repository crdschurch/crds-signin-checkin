import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { SearchComponent } from './search.component';
import { ResultsComponent } from './results';
import { NoresultsComponent } from './results';

import { routing } from './child-checkin.routes';

@NgModule({
  declarations: [
    AssignmentComponent,
    SearchComponent,
    ResultsComponent,
    NoresultsComponent
  ],
  imports: [
    CommonModule,
    routing
  ],
  exports: [
    SearchComponent
  ]
})

export class SearchModule { }
