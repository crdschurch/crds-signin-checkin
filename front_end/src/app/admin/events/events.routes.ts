import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { EventListComponent } from './event-list.component';
import { RoomListComponent } from './rooms/room-list.component';
import { RoomGroupListComponent } from './rooms/room-group-list.component';
import { EventImportComponent } from './import/event-import.component';
import { EventResetComponent } from './reset/event-reset.component';

export const eventsRoutes: Routes = [
  { path: '', component: EventListComponent },
  { path: ':eventId/rooms', component: RoomListComponent },
  { path: ':eventId/rooms/:roomId', component: RoomGroupListComponent },
  { path: ':eventId/import', component: EventImportComponent },
  { path: ':eventId/reset', component: EventResetComponent }
];

export const eventsRouting: ModuleWithProviders = RouterModule.forChild(eventsRoutes);
