import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';

import { SearchComponent } from './search.component';
import { NumberPadComponent } from './number-pad';

@NgModule({
  declarations: [
    SearchComponent,
    NumberPadComponent
  ],
  imports: [
    SharedModule,
  ],
  exports: [
    SearchComponent,
    NumberPadComponent
  ]
})

export class SearchModule { }
