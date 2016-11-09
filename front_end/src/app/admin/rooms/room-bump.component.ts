import { Component, Input, OnInit } from '@angular/core';
import { Room } from '../../shared/models';

@Component({
  selector: '.room-bump',
  templateUrl: 'room-bump.component.html',
  styleUrls: ['room-bump.component.scss']
})
export class RoomBumpComponent implements OnInit {
  @Input() room: Room;

  ngOnInit() {
    console.log("rb on init")
  }
}
