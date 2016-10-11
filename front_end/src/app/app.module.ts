import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import { HttpClientService } from './shared/services';
import { AdminModule } from './admin';
import { ChildCheckinModule } from './child-checkin';
import { HomeModule } from './home';
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
    ChildCheckinModule,
    AdminModule,
    routing
  ],
  providers: [
    FormsModule,
    HttpClientService
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
