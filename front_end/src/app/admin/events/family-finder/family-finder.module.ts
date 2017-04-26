import { NgModule } from '@angular/core';
import { TextMaskModule } from 'angular2-text-mask';

import { SharedModule } from '../../../shared/shared.module';
import { HeaderModule } from '../../header';
import { ChildSigninCardModule } from '../../../shared/components/child-signin-card';
import { FamilyFinderComponent } from './family-finder.component';
import { HouseholdComponent } from './household/household.component';
import { HouseholdEditComponent } from './household/edit/household-edit.component';
import { ServingToggleModule } from '../../../child-signin/serving/serving-toggle.module';
import { familyFinderRouting } from './family-finder.routes';

@NgModule({
  declarations: [
    FamilyFinderComponent,
    HouseholdComponent,
    HouseholdEditComponent
  ],
  imports: [
    familyFinderRouting,
    ChildSigninCardModule,
    HeaderModule,
    TextMaskModule,
    SharedModule,
    ServingToggleModule
  ],
  exports: [ ]
})

export class FamilyFinderModule { }
