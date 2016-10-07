import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { HttpClient } from './shared/http-client.service';
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
    HttpClient,
    HomeModule,
    ChildCheckinModule,
    AdminModule,
    routing
  ],
  providers: [
    FormsModule,
    HttpClient
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
