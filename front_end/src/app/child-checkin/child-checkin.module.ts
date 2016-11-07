import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';

import { ChildCheckinComponent } from './child-checkin.component';
import { RoomComponent } from './room';;
import { RoomChildComponent } from './room/child';
import { childCheckinRouting } from './child-checkin.routes';

@NgModule({
  declarations: [
    ChildCheckinComponent,
    RoomComponent,
    RoomChildComponent
  ],
  imports: [
    SharedModule,
    childCheckinRouting
  ],
})

export class ChildCheckinModule { }
