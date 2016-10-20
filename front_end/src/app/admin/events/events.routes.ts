import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { EventListComponent } from './event-list.component';
import { RoomsComponent } from '../rooms/rooms.component';

export const eventsRoutes: Routes = [
  {
    path: '', component: EventListComponent,
    // children: [
    //   { path: ':eventId/rooms', component: RoomsComponent }
    // ]
  }
];

export const eventsRouting: ModuleWithProviders = RouterModule.forChild(eventsRoutes);
