import { Component, Input, OnInit } from '@angular/core';
import { Room } from '../../../shared/models';

@Component({
  selector: '.room-bump',
  templateUrl: 'room-bump.component.html',
  styleUrls: ['room-bump.component.scss']
})
export class RoomBumpComponent implements OnInit {
  @Input() room: Room;
  @Input() bumpingRooms: Room[];

  ngOnInit() {
  }

  unBump() {
    delete this.room.BumpingRuleId;
    delete this.room.BumpingRulePriority;
  }
  bump() {
    this.room.BumpingRulePriority = ++this.bumpingRooms.length;
  }
  bumpUp() {

  }
  bumpDown() {

  }

}
