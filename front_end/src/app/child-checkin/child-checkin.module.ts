import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';

import { ChildCheckinComponent } from './child-checkin.component';
import { RoomComponent } from './room';
import { childCheckinRouting } from './child-checkin.routes';

@NgModule({
  declarations: [
    ChildCheckinComponent,
    RoomComponent
  ],
  imports: [
    SharedModule,
    childCheckinRouting
  ],
})

export class ChildCheckinModule { }
