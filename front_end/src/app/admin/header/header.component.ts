import { Component } from '@angular/core';
import { HeaderService } from './header.service';
import { Event } from '../events/event';

@Component({
  selector: 'header-event',
  templateUrl: 'header.component.html',
  styleUrls: ['header.component.scss'],
  providers: [ ]
})
export class HeaderComponent {
  event: any;

  constructor(private headerService: HeaderService) {
    headerService.eventAnnounced$.subscribe(
      event => {
        console.log("announce event admin component")
        this.event = event
      });
  }
}
