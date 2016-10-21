import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MomentModule } from 'angular2-moment';
import { ReactiveFormsModule } from '@angular/forms';

import { EventListComponent } from './event-list.component';
import { RoomListComponent } from '../rooms/room-list.component';
import { RoomGroupListComponent } from '../rooms/room-group-list.component';
import { RoomComponent } from '../rooms/room.component';
import { eventsRouting } from './events.routes';

@NgModule({
  declarations: [
    EventListComponent,
    RoomListComponent,
    RoomGroupListComponent,
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
    RoomComponent
  ]
})

export class EventsModule { }
