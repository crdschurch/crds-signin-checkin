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
  @Input() index: number;

  ngOnInit() {
  }

  isFirst() { return this.index === 0; }
  isLast() { return this.index === this.bumpingRooms.length - 1; }
  isBumped() { return this.room.isBumpingRoom(); }
  isUnBumped() { return !this.room.isBumpingRoom(); }

  unBump() {
    delete this.room.BumpingRuleId;
    delete this.room.BumpingRulePriority;
    let i = 0;
    let j = 0;
    for (let room of this.bumpingRooms) {
      if (this.index !== i) {
        room.BumpingRulePriority = ++j;
      }
      i++;
    }
  }
  bump() {
    if (!this.room.BumpingRulePriority) {
      this.room.BumpingRulePriority = ++this.bumpingRooms.length;
    }
  }
  bumpUp() {
    let i = 0;
    for (let room of this.bumpingRooms) {
      if (this.index === 0) {
        break;
      } else if (i === this.index) {
        room.BumpingRulePriority--;
      } else if (i === this.index - 1) {
        room.BumpingRulePriority++;
      }
      i++;
    }
  }
  bumpDown() {
    let i = 0;
    for (let room of this.bumpingRooms) {
      if (this.index === this.bumpingRooms.length - 1) {
        break;
      } else if (i === this.index) {
        room.BumpingRulePriority++;
      } else if (i === this.index + 1) {
        room.BumpingRulePriority--;
      }
      i++;
    }
  }

}
