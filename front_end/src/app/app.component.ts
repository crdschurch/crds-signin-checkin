import { Component, ViewEncapsulation, ViewContainerRef, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToasterModule, ToasterService, ToasterConfig } from 'angular2-toaster/angular2-toaster';
import { ContentService, RootService, SetupService, HttpClientService } from './shared/services';
import { ChannelService, ConnectionState } from './shared/services';
import { ApiService, UserService } from './shared/services';

import { ComponentsHelper } from 'ng2-bootstrap/ng2-bootstrap';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [ToasterModule, ToasterService, ContentService, SetupService, HttpClientService, ApiService, ChannelService]
})
export class AppComponent implements OnInit {

  private viewContainerRef: ViewContainerRef;

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
              private userService: UserService,
              private channelService: ChannelService) {
    // You need this small hack in order to catch application root view container ref
    this.viewContainerRef = viewContainerRef;

    this.hackToFixNg221BugForNg2Bootstrap();

    rootService.eventAnnounced$.subscribe(
      event => {
        this.addToast(event);
      });



  }

  ngOnInit() {
    // Start the signalr connection up!
    // TODO - commenting out to try and track down ws/apache issues
    console.log('Starting the channel service');
    this.channelService.start();

    this.contentService.loadData();
  }

  inRoom() {
    return this.router.url === '/child-signin/room';
  }

  addToast(contentBlockTitle) {
    this.contentService.getToastContent(contentBlockTitle).then((toast) => {
      this.toasterService.pop(toast);
    });
  }

  // https://github.com/valor-software/ng2-bootstrap/issues/986#issuecomment-262293199
  private hackToFixNg221BugForNg2Bootstrap(): void {
    ComponentsHelper.prototype.getRootViewContainerRef = function () {
      // https://github.com/angular/angular/issues/9293
      if (this.root) {
        return this.root;
      }
      let comps = this.applicationRef.components;
      if (!comps.length) {
        throw new Error('ApplicationRef instance not found');
      }
      try {
        /* one more ugly hack, read issue above for details */
        let rootComponent = this.applicationRef._rootComponents[0];
        // this.root = rootComponent._hostElement.vcRef;
        this.root = rootComponent._component.viewContainerRef;
        return this.root;
      } catch (e) {
        throw new Error('ApplicationRef instance not found');
      }
    };
  }
}
