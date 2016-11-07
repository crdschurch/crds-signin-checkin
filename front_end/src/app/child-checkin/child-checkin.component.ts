import { Component } from '@angular/core';

import { ChildCheckinService } from './child-checkin.service';

@Component({
  selector: 'child-checkin',
  templateUrl: 'child-checkin.component.html',
  styleUrls: ['child-checkin.component.scss', 'scss/_stepper.scss' ],
  providers: [ ChildCheckinService ]
})
export class ChildCheckinComponent {

  constructor() {}
}
