import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import { ChildCheckinModule } from './child-checkin/child-checkin.module';
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
    ChildCheckinModule,
    routing
  ],
  providers: [
    FormsModule,
    HttpModule
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule { }
