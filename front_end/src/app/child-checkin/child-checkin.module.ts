import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { MomentModule } from 'angular2-moment';

import { AdminService } from '../admin/admin.service';
import { SetupService } from '../setup/setup.service';
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
    childCheckinRouting,
    MomentModule
  ],
  providers: [
    AdminService,
    SetupService
  ]
})

export class ChildCheckinModule { }
