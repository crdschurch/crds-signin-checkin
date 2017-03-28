import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { ChildSigninComponent } from './child-signin.component';
import { MomentModule } from 'angular2-moment';

@NgModule({
  imports: [BrowserModule, CommonModule, FormsModule, MomentModule],
  exports: [ChildSigninComponent],
  declarations: [ChildSigninComponent],
  providers: [],
})
export class ChildSigninModule { }
