import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { EventListComponent } from './event-list.component';
import { RoomListComponent } from '../rooms/room-list.component';
import { RoomGroupListComponent } from '../rooms/room-group-list.component';
import { EventImportComponent } from './import/event-import.component';
import { EventResetComponent } from './reset/event-reset.component';
import { CanActivateIfLoggedInGuard } from '../../shared/guards';

export const eventsRoutes: Routes = [
  { path: '', component: EventListComponent, canActivate: [CanActivateIfLoggedInGuard] },
  { path: ':eventId/rooms', component: RoomListComponent, canActivate: [CanActivateIfLoggedInGuard] },
  { path: ':eventId/rooms/:roomId', component: RoomGroupListComponent, canActivate: [CanActivateIfLoggedInGuard] },
  { path: ':eventId/import', component: EventImportComponent, canActivate: [CanActivateIfLoggedInGuard] },
  { path: ':eventId/reset', component: EventResetComponent, canActivate: [CanActivateIfLoggedInGuard] }
];

export const eventsRouting: ModuleWithProviders = RouterModule.forChild(eventsRoutes);
