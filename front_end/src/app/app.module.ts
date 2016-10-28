import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { MomentModule } from 'angular2-moment';
import { ToasterModule } from 'angular2-toaster/angular2-toaster';

import { CookieService } from 'angular2-cookie/services/cookies.service';

import { HttpClientService } from './shared/services';
import { ContentService, RootService } from './shared/services';
import { AdminModule } from './admin';
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
    HttpClientService,
    ContentService,
    RootService,
    ToasterModule
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule {

}
