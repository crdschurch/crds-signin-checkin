import { Component, Input, Output, EventEmitter } from '@angular/core';

import { Child } from '../../../shared/models/child';

@Component({
  selector: 'room-child',
  templateUrl: 'room-child.component.html'
})

export class RoomChildComponent {
  @Input() child: Child;
  @Output() toggleCheckIn: EventEmitter<any> = new EventEmitter();

  constructor() {}

  toggle() {
    this.toggleCheckIn.emit(this.child);
  }

  getCallNumber() {
    return this.child.callNumber().toString();
  }
}
