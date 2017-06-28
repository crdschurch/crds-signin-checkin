import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { FamilyFinderComponent } from './family-finder.component';
import { HouseholdComponent } from './household/household.component';
import { HouseholdEditComponent } from './household/edit/household-edit.component';
import { FamilyMemberEntryComponent } from './household/family-member-entry/family-member-entry.component';


export const familyFinderRoutes: Routes = [
  { path: '', component: FamilyFinderComponent },
  { path: ':householdId', component: HouseholdComponent },
  { path: ':householdId/edit', component: HouseholdEditComponent },
  { path: ':householdId/entry', component: FamilyMemberEntryComponent }
];

export const familyFinderRouting: ModuleWithProviders = RouterModule.forChild(familyFinderRoutes);
