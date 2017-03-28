import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { ChildSigninCardComponent } from './child-signin-card.component';
import { MomentModule } from 'angular2-moment';

@NgModule({
  imports: [BrowserModule, CommonModule, FormsModule, MomentModule],
  exports: [ChildSigninCardComponent],
  declarations: [ChildSigninCardComponent],
  providers: [],
})
export class ChildSigninCardModule { }
