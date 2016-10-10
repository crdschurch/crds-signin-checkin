import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

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
    HomeModule,
    ChildCheckinModule,
    AdminModule,
    routing
  ],
  providers: [
    FormsModule,
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
