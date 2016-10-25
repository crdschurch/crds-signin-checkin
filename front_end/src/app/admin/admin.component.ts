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
  theString: string;
  constructor(private missionService: HeaderService) {
    missionService.missionAnnounced$.subscribe(
      astronaut => {
        this.theString = `${astronaut} confirmed the mission`
        console.log(`${astronaut} confirmed the mission`)
      });
  }
}
