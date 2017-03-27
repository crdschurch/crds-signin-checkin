import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { FamilyFinderComponent } from './family-finder.component';
import { HouseholdComponent } from './household/household.component';

export const familyFinderRoutes: Routes = [
  { path: '', component: FamilyFinderComponent },
  { path: ':householdId', component: HouseholdComponent }
];

export const familyFinderRouting: ModuleWithProviders = RouterModule.forChild(familyFinderRoutes);
