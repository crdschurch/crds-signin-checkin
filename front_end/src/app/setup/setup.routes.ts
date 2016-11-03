import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SetupComponent } from './setup.component';

const setupRoutes: Routes = [
  {
    path: 'setup', component: SetupComponent,
  }
];

export const setupRouting: ModuleWithProviders = RouterModule.forChild(setupRoutes);
