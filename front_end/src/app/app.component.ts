import { Component, ViewEncapsulation, ViewContainerRef, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToasterModule, ToasterService, ToasterConfig } from 'angular2-toaster/angular2-toaster';
import { ContentService, RootService } from './shared/services';
import { SetupService } from './setup/setup.service';

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
  encapsulation: ViewEncapsulation.None,
  providers: [ToasterModule, ToasterService, ContentService, SetupService]
})
export class AppComponent implements OnInit {

  private viewContainerRef: ViewContainerRef;

  public customToasterConfig: ToasterConfig =
    new ToasterConfig({
      positionClass: 'toast-top-center'
    });

    event: any;

  constructor(private router: Router,
              viewContainerRef: ViewContainerRef,
              private contentService: ContentService,
              private toasterService: ToasterService,
              private setupService: SetupService,
              private rootService: RootService) {

    // You need this small hack in order to catch application root view container ref
    this.viewContainerRef = viewContainerRef;

    rootService.eventAnnounced$.subscribe(
      event => {
        this.addToast(event);
      });
  }

  ngOnInit() {
    this.contentService.loadData();
  }

  activeStep1() {
    return this.router.url === '/child-signin';
  }

  activeStep2() {
    return this.router.url === '/child-signin/results';
  }

  activeStep3() {
    return this.router.url === '/child-signin/assignment';
  }

  inRoom() {
    return this.router.url === '/child-signin/room';
  }

  addToast(contentBlockTitle) {
    this.contentService.getToastContent(contentBlockTitle).then((toast) => {
      this.toasterService.pop(toast);
    });
  }

}
