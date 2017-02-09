import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { MomentModule } from 'angular2-moment';

import { ApiService, SetupService } from '../shared/services';
import { ChildCheckinComponent } from './child-checkin.component';
import { RoomComponent } from './room';
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
    childCheckinRouting,
    MomentModule
  ],
  providers: [
    ApiService,
    SetupService
  ]
})

export class ChildCheckinModule { }
