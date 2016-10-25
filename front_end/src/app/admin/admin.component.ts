import { Component, ViewEncapsulation } from '@angular/core';
import { HeaderService } from './header/header.service';
import { Event } from './events/event';

@Component({
  selector: 'admin',
  templateUrl: 'admin.component.html',
  styleUrls: ['admin.component.scss'],
  providers: [ HeaderService ],
  encapsulation: ViewEncapsulation.None
})
export class AdminComponent {
  event: any;
  mission: any;
  constructor(private headerService: HeaderService) {
    headerService.eventAnnounced$.subscribe(
      event => {
        console.log("announce event admin component")
        this.event = event
      });
    headerService.missionAnnounced$.subscribe(
      mission => {
        console.log("announce mission admin component")
        this.mission = event
      });
  }
}
