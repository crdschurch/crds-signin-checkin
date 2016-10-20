import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { EventListComponent } from './event-list.component';
import { RoomListComponent } from '../rooms/room-list.component';
import { RoomComponent } from '../rooms/room.component';

export const eventsRoutes: Routes = [
  { path: '', component: EventListComponent },
  { path: ':eventId/rooms', component: RoomListComponent },
  { path: ':eventId/rooms/:roomId', component: RoomComponent }
];

export const eventsRouting: ModuleWithProviders = RouterModule.forChild(eventsRoutes);
