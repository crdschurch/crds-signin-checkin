import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MomentModule } from 'angular2-moment';

import { EventListComponent } from './event-list.component';
import { RoomsComponent } from '../rooms/rooms.component';
import { RoomComponent } from '../rooms/room.component';
import { eventsRouting } from './events.routes';

@NgModule({
  declarations: [
    EventListComponent,
    RoomsComponent
  ],
  imports: [
    eventsRouting,
    SharedModule,
    MomentModule
  ],
  exports: [
    EventListComponent,
    RoomsComponent
  ]
})

export class EventsModule { }
