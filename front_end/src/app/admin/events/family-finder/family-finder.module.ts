import { NgModule } from '@angular/core';
import { TextMaskModule } from 'angular2-text-mask';

import { SharedModule } from '../../../shared/shared.module';
import { HeaderModule } from '../../header';
import { FamilyFinderComponent } from './family-finder.component';
import { familyFinderRouting } from './family-finder.routes';

@NgModule({
  declarations: [
    FamilyFinderComponent,
  ],
  imports: [
    familyFinderRouting,
    HeaderModule,
    TextMaskModule,
    SharedModule
  ],
  exports: [ ]
})

export class FamilyFinderModule { }
