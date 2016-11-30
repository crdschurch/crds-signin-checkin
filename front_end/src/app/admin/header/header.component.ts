import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HeaderService } from './header.service';
import { Event } from '../../shared/models';
import { UserService } from '../../shared/services';

@Component({
  selector: 'header-event',
  templateUrl: 'header.component.html',
  styleUrls: ['header.component.scss']
})
export class HeaderComponent {
  event: Event;

  constructor(private headerService: HeaderService,
              private userService: UserService,
              private router: Router) {
    headerService.eventAnnounced$.subscribe(
      event => {
        this.event = event;
      });
  }

  logOut() {
    this.userService.logOut();
    this.router.navigate(['/admin/sign-in']);
  }
}
