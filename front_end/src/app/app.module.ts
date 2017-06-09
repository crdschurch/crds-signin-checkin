import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { MomentModule } from 'angular2-moment';
import { ToasterModule } from 'angular2-toaster/angular2-toaster';
import { Angulartics2Module, Angulartics2GoogleTagManager } from 'angulartics2';
import './rxjs-operators';

import { CookieService } from 'angular2-cookie/services/cookies.service';

import { ContentService, RootService, UserService, ChannelConfig, SignalrWindow } from './shared/services';
import { CanActivateIfLoggedInGuard } from './shared/guards';
import { AdminModule } from './admin';
import { ChildCheckinModule } from './child-checkin';
import { ChildSigninModule } from './child-signin';
import { HomeModule } from './home';
import { SetupModule } from './setup';
import { AppComponent } from './app.component';
import { routing } from './app.routes';
import { PreloaderModule } from './shared/preloader';

let channelConfig = new ChannelConfig();
channelConfig.url = `${process.env.SIGNALR_ENDPOINT}`;
channelConfig.hubName = 'EventHub';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    Angulartics2Module.forRoot([Angulartics2GoogleTagManager]),
    BrowserModule,
    FormsModule,
    HttpModule,
    HomeModule,
    SetupModule,
    ChildCheckinModule,
    ChildSigninModule,
    AdminModule,
    routing,
    MomentModule,
    ToasterModule,
    PreloaderModule
  ],
  exports: [
    ToasterModule
  ],
  providers: [
    FormsModule,
    CookieService,
    ContentService,
    RootService,
    ToasterModule,
    UserService,
    CanActivateIfLoggedInGuard,
    { provide: SignalrWindow, useValue: window },
    { provide: 'channel.config', useValue: channelConfig }
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule {

}
