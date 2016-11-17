import { Component, ViewEncapsulation, ViewContainerRef, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToasterModule, ToasterService, ToasterConfig } from 'angular2-toaster/angular2-toaster';
import { ContentService, RootService, SetupService, HttpClientService, ApiService, UserService } from './shared/services';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [ToasterModule, ToasterService, ContentService, SetupService, HttpClientService, ApiService]
})
export class AppComponent implements OnInit {

  private viewContainerRef: ViewContainerRef;
  private kioskDisplay: Array<string> = [];
  private loggedInDisplay: string;
  private displayHelp: boolean = false;

  public customToasterConfig: ToasterConfig =
    new ToasterConfig({
      positionClass: 'toast-top-full-width',
      timeout: 3000
    });

    event: any;

  constructor(private router: Router,
              viewContainerRef: ViewContainerRef,
              private contentService: ContentService,
              private toasterService: ToasterService,
              private setupService: SetupService,
              private rootService: RootService,
              private userService: UserService) {

    // You need this small hack in order to catch application root view container ref
    this.viewContainerRef = viewContainerRef;

    rootService.eventAnnounced$.subscribe(
      event => {
        this.addToast(event);
      });
  }

  ngOnInit() {
    this.contentService.loadData();
    if (process.env.ENV !== 'PRODUCTION' && process.env.ENV !== 'DEMO') {
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
  }

  inRoom() {
    return this.router.url === '/child-signin/room';
  }

  addToast(contentBlockTitle) {
    this.contentService.getToastContent(contentBlockTitle).then((toast) => {
      this.toasterService.pop(toast);
    });
  }

  closeHelp() {
    this.displayHelp = false;
  }

}
