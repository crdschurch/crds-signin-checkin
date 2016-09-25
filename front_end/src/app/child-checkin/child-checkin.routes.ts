import { ModuleWithProviders } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const appRoutes: Routes = [
  { path: 'child-checkin', redirectTo: '/search', pathMatch: 'full' },
  { path: 'search', component: SearchComponent },
  { path: 'results', component: ResultsComponent },
  { path: 'room', component: RoomComponent },
  { path: 'assignment', component: AssignmentComponent },
  { path: 'guest', component: GuestComponent }
  { path: 'hero/:id', component: HeroDetailComponent },
  { path: 'crisis-center', component: CrisisCenterComponent },
  {
];

export const routing: ModuleWithProviders = RouterModule.forRoot(appRoutes);
