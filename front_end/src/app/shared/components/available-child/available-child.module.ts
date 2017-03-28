import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { AvailableChildComponent } from './available-child.component';
import { MomentModule } from 'angular2-moment';

@NgModule({
  imports: [BrowserModule, CommonModule, FormsModule, MomentModule],
  exports: [AvailableChildComponent],
  declarations: [AvailableChildComponent],
  providers: [],
})
export class AvailableChildModule { }
