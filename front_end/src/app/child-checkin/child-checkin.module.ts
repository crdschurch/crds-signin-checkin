import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';

import { ChildCheckinComponent } from './child-checkin.component';
import { SearchComponent } from './search';
import { AssignmentComponent } from './assignment';
import { childCheckinRouting } from './child-checkin.routes';

@NgModule({
  declarations: [
    ChildCheckinComponent,
    SearchComponent,
    AssignmentComponent,
  ],
  imports: [
    CommonModule,
    childCheckinRouting
  ],
})

export class ChildCheckinModule { }
