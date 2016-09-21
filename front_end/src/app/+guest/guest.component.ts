import { Component, OnInit } from '@angular/core';
import { ROUTER_DIRECTIVES } from '@angular/router';

@Component({
  moduleId: module.id,
  selector: 'app-guest',
  templateUrl: 'guest.component.html',
  styleUrls: ['guest.component.css'],
  directives: [ROUTER_DIRECTIVES]
})
export class GuestComponent implements OnInit {

  constructor() {}

  ngOnInit() {
  }

}
