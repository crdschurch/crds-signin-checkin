import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';

import { ChildCheckinComponent } from './child-checkin.component';
import { SearchModule } from './search';
import { ResultsComponent } from './results';
import { GuestComponent } from './guest';
import { AssignmentComponent } from './assignment';
import { childCheckinRouting } from './child-checkin.routes';

@NgModule({
  declarations: [
    ChildCheckinComponent,
    ResultsComponent,
    GuestComponent,
    AssignmentComponent,
  ],
  imports: [
    SharedModule,
    SearchModule,
    childCheckinRouting
  ],
})

export class ChildCheckinModule { }
