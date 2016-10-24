import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MomentModule } from 'angular2-moment';
import { ReactiveFormsModule } from '@angular/forms';

import { EventListComponent } from './event-list.component';
import { RoomListComponent } from '../rooms/room-list.component';
import { RoomGroupListComponent } from '../rooms/room-group-list.component';
import { RoomGroupComponent } from '../rooms/room-group.component';
import { RoomComponent } from '../rooms/room.component';
import { eventsRouting } from './events.routes';

@NgModule({
  declarations: [
    EventListComponent,
    RoomListComponent,
    RoomGroupListComponent,
    RoomGroupComponent,
    RoomComponent
  ],
  imports: [
    eventsRouting,
    SharedModule,
    MomentModule,
    ReactiveFormsModule
  ],
  exports: [
    EventListComponent,
    RoomListComponent,
    RoomGroupListComponent,
    RoomGroupComponent,
    RoomComponent
  ]
})

export class EventsModule { }
