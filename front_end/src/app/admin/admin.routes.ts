import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AdminComponent } from './admin.component';
import { SignInComponent } from './sign-in';

const adminRoutes: Routes = [
  {
    path: 'admin', component: AdminComponent,
    children: [
      { path: 'sign-in', component: SignInComponent }
    ]
  }
];

export const adminRouting: ModuleWithProviders = RouterModule.forChild(adminRoutes);
