import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AdminComponent } from './admin.component';
import { SignInComponent } from './sign-in';
import { ReportsComponent } from './reports';
import { eventsRoutes } from './events/events.routes';
import { CanActivateIfLoggedInGuard } from '../shared/guards';

const adminRoutes: Routes = [
  {
    path: 'admin', component: AdminComponent,
    children: [
      { path: 'dashboard', redirectTo: 'events' },
      { path: 'sign-in', component: SignInComponent },
      { path: 'reports', component: ReportsComponent },
      { path: 'events', children: [...eventsRoutes], canActivate: [CanActivateIfLoggedInGuard] }
    ]
  }
];

export const adminRouting: ModuleWithProviders = RouterModule.forChild(adminRoutes);
