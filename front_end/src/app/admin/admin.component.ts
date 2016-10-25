import { Component, ViewEncapsulation } from '@angular/core';
import { HeaderService } from './header/header.service';

@Component({
  selector: 'admin',
  templateUrl: 'admin.component.html',
  styleUrls: ['admin.component.scss'],
  providers: [ HeaderService ],
  encapsulation: ViewEncapsulation.None
})
export class AdminComponent {
  constructor(private missionService: HeaderService) {
    missionService.missionConfirmed$.subscribe(
      astronaut => {
        // this.history.push(`${astronaut} confirmed the mission`);
        console.log(`${astronaut} confirmed the mission`)
      });
  }
}
