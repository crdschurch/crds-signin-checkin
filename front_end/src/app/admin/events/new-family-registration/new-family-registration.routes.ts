import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { NewFamilyRegistrationComponent } from './new-family-registration.component'

export const newFamilyRegistrationRoutes: Routes = [
  { path: '', component: NewFamilyRegistrationComponent }
];

export const newFamilyRegistrationRouting: ModuleWithProviders = RouterModule.forChild(newFamilyRegistrationRoutes);
