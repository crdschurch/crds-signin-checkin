import { Component, Input } from '@angular/core';

import { Child } from '../../../shared/models/child';

@Component({
  selector: 'room-child',
  templateUrl: 'room-child.component.html',
})

export class RoomChildComponent {
  @Input() child: Child;

  constructor() {}
}
