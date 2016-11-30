import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { MomentModule } from 'angular2-moment';
import { ReactiveFormsModule } from '@angular/forms';

import { HeaderModule } from '../header';
import { NewFamilyRegistrationComponent } from './new-family-registration';
import { EventImportComponent } from './import/event-import.component';
import { EventListComponent } from './event-list.component';
import { EventResetComponent } from './reset/event-reset.component';
import { RoomListComponent } from './rooms/room-list.component';
import { RoomGroupListComponent } from './rooms/room-group-list.component';
import { RoomGroupComponent } from './rooms/room-group.component';
import { RoomComponent } from './rooms/room.component';
import { RoomBumpComponent } from './rooms/alternate-rooms/room-bump.component';
import { AlternateRoomsComponent } from './rooms/alternate-rooms/alternate-rooms.component';
import { eventsRouting } from './events.routes';

@NgModule({
  declarations: [
    EventImportComponent,
    EventListComponent,
    EventResetComponent,
    RoomListComponent,
    RoomGroupListComponent,
    RoomGroupComponent,
    RoomComponent,
    AlternateRoomsComponent,
    NewFamilyRegistrationComponent,
    RoomBumpComponent
  ],
  imports: [
    eventsRouting,
    SharedModule,
    MomentModule,
    HeaderModule,
    ReactiveFormsModule
  ],
  exports: [ ]
})

export class EventsModule { }
