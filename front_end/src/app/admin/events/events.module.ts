import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MomentModule } from 'angular2-moment';
import { ReactiveFormsModule } from '@angular/forms';

import { EventListComponent } from './event-list.component';
import { RoomListComponent } from '../rooms/room-list.component';
import { RoomComponent } from '../rooms/room.component';
import { eventsRouting } from './events.routes';

@NgModule({
  declarations: [
    EventListComponent,
    RoomListComponent,
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
    RoomComponent
  ]
})

export class EventsModule { }
