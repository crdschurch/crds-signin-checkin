import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { HeaderService } from './header.service';
import { Event } from '../../shared/models';
import { UserService, SetupService } from '../../shared/services';
import * as moment from 'moment';

@Component({
  selector: 'header-event',
  templateUrl: 'header.component.html',
  styleUrls: ['header.component.scss'],
})
export class HeaderComponent {
  event: Event;
  private kioskDisplay: Array<string> = [];
  private loggedInDisplay: string;
  private displayHelp = false;

  private headerClicks = 0;
  private firstClick;
  private lastClick;

  public isCollapsed = true;

  public collapsed(event) {
  }

  public expanded(event) {
  }

  constructor(private headerService: HeaderService,
              private userService: UserService,
              private setupService: SetupService,
              private router: Router) {
    headerService.eventAnnounced$.subscribe(
      event => {
        this.event = event;
      });
  }

  click() {
    this.headerClicks++;
    if (this.headerClicks === 1) {
      this.firstClick = new Date();
    } else if (this.headerClicks === 5) {
      this.lastClick = new Date();
    }
    const durationSeconds = moment.duration(moment(this.lastClick).diff(moment(this.firstClick))).asSeconds();
    if (this.headerClicks === 5) {
      if (durationSeconds <= 5) {
        this.showConfigDetails();
      }
      this.clearClicker();
    }
  }

  clearClicker() {
    this.firstClick = null;
    this.lastClick = null;
    this.headerClicks = 0;
  }

  showConfigDetails() {
    this.displayHelp = true;
    if (this.setupService.getMachineIdConfigCookie()) {
      this.kioskDisplay = [`Kiosk Config Id: ${this.setupService.getMachineIdConfigCookie()}`,
        `Kiosk Name: ${this.setupService.getMachineDetailsConfigCookie().KioskName}`,
        `Kiosk Type: ${this.setupService.getMachineDetailsConfigCookie().kioskType()}`,
        `Kiosk Site Id: ${this.setupService.getMachineDetailsConfigCookie().CongregationId}`,
        `Kiosk Site Name: ${this.setupService.getMachineDetailsConfigCookie().CongregationName}`,
        `Kiosk Room Id: ${this.setupService.getMachineDetailsConfigCookie().RoomId}`,
        `Kiosk Room Name: ${this.setupService.getMachineDetailsConfigCookie().RoomName}`];
    }
    this.loggedInDisplay = `User Logged In: ${this.userService.isLoggedIn()}`;
  }

  closeHelp() {
    this.displayHelp = false;
  }

  logOut() {
    this.userService.logOut();
    this.router.navigate(['/admin/sign-in']);
  }

  isEventsRoute() {
    return this.event !== undefined;
  }
}
