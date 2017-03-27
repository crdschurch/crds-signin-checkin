import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { FamilyFinderComponent } from './family-finder.component'

export const familyFinderRoutes: Routes = [
  { path: '', component: FamilyFinderComponent }
];

export const familyFinderRouting: ModuleWithProviders = RouterModule.forChild(familyFinderRoutes);
