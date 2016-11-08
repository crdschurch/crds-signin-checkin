import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MomentModule } from 'angular2-moment';
import { ReactiveFormsModule } from '@angular/forms';

import { EventImportComponent } from './import/event-import.component';
import { EventListComponent } from './event-list.component';
import { EventResetComponent } from './reset/event-reset.component';
import { RoomListComponent } from '../rooms/room-list.component';
import { RoomGroupListComponent } from '../rooms/room-group-list.component';
import { RoomGroupComponent } from '../rooms/room-group.component';
import { RoomComponent } from '../rooms/room.component';
import { HeaderComponent } from '../header/header.component';
import { eventsRouting } from './events.routes';

@NgModule({
  declarations: [
    EventImportComponent,
    EventListComponent,
    EventResetComponent,
    RoomListComponent,
    RoomGroupListComponent,
    RoomGroupComponent,
    HeaderComponent,
    RoomComponent
  ],
  imports: [
    eventsRouting,
    SharedModule,
    MomentModule,
    ReactiveFormsModule
  ],
  exports: [ ]
})

export class EventsModule { }
