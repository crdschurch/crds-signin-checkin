import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { EventListComponent } from './event-list.component';
import { RoomsComponent } from '../rooms/rooms.component';
import { RoomComponent } from '../rooms/room.component';

export const eventsRoutes: Routes = [
  { path: '', component: EventListComponent },
  { path: ':eventId/rooms', component: RoomsComponent },
  { path: ':eventId/rooms/:roomId', component: RoomComponent }
];

export const eventsRouting: ModuleWithProviders = RouterModule.forChild(eventsRoutes);
