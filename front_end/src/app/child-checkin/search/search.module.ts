import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';

import { SearchComponent } from './search.component';
import { NumberPadComponent } from './number-pad';
import { searchRouting } from './search.routes';


@NgModule({
  declarations: [
    SearchComponent,
    NumberPadComponent
  ],
  imports: [
    SharedModule,
    searchRouting
  ],
  exports: [
    SearchComponent,
    NumberPadComponent
  ]
})

export class SearchModule { }
