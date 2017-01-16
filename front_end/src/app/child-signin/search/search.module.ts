import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';

import { SearchComponent } from './search.component';
import { searchRouting } from './search.routes';

@NgModule({
  declarations: [
    SearchComponent
  ],
  imports: [
    SharedModule,
    searchRouting
  ],
  exports: [
    SearchComponent
  ]
})

export class SearchModule { }
