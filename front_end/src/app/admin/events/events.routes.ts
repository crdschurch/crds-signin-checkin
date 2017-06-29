import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { EventListComponent } from './event-list.component';
import { EventTemplatesListComponent } from './templates/event-list.component';
import { RoomListComponent } from './rooms/room-list.component';
import { RoomGroupListComponent } from './rooms/room-group-list.component';
import { EventImportComponent } from './import/event-import.component';
import { EventResetComponent } from './reset/event-reset.component';
import { ManageChildrenComponent } from './manage-children/manage-children.component';
import { newFamilyRegistrationRoutes } from './new-family-registration/new-family-registration.routes';
import { familyFinderRoutes } from './family-finder/family-finder.routes';
import { CanDeactivateGuard } from '../../shared/guards';

export const eventsRoutes: Routes = [
  { path: '', component: EventListComponent },
  { path: 'templates', component: EventTemplatesListComponent },
  { path: ':eventId/rooms', component: RoomListComponent, canDeactivate: [ CanDeactivateGuard ]},
  { path: ':eventId/rooms/:roomId', component: RoomGroupListComponent },
  { path: ':eventId/import/events', component: EventImportComponent,  data: { template: false } },
  { path: ':eventId/import/templates', component: EventImportComponent,  data: { template: true } },
  { path: ':eventId/children', component: ManageChildrenComponent },
  { path: ':eventId/reset', component: EventResetComponent },
  { path: ':eventId/new-family-registration', children: [...newFamilyRegistrationRoutes]},
  { path: ':eventId/family-finder', children: [...familyFinderRoutes]}
];
export const eventsRouting: ModuleWithProviders = RouterModule.forChild(eventsRoutes);
