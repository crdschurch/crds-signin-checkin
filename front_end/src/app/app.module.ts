import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { MomentModule } from 'angular2-moment';
import { ToasterModule } from 'angular2-toaster/angular2-toaster';
import './rxjs-operators';

import { CookieService } from 'angular2-cookie/services/cookies.service';

import { ContentService, RootService, UserService } from './shared/services';
import { CanActivateIfLoggedInGuard } from './shared/guards';
import { AdminModule } from './admin';
import { ChildCheckinModule } from './child-checkin';
import { ChildSigninModule } from './child-signin';
import { HomeModule } from './home';
import { SetupModule } from './setup';
import { AppComponent } from './app.component';
import { routing } from './app.routes';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
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
    ToasterModule
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
    CanActivateIfLoggedInGuard
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule {

}
