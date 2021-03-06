import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Room } from '../../../../shared/models';
import { AdminService } from '../../../admin.service';

@Component({
  selector: '.room-bump',
  templateUrl: 'room-bump.component.html',
  styleUrls: ['room-bump.component.scss']
})
export class RoomBumpComponent {
  @Input() room: Room;
  @Input() bumpingRooms: Room[];
  @Input() allRooms: Room[];
  @Input() index: number;
  @Input() selectedBumpingType: number;
  @Output() setDirtyChild = new EventEmitter<Room[]>();

  constructor( private adminService: AdminService,
               private route: ActivatedRoute) {}

  isFirst() { return this.room.isBumpingRoom() && this.index === 0; }
  isLast() { return this.room.isBumpingRoom() && this.index === this.bumpingRooms.length - 1; }
  isBumped() { return this.room.isBumpingRoom(); }
  isUnBumped() { return !this.room.isBumpingRoom(); }

  unBump() {
    delete this.room.BumpingRuleId;
    delete this.room.BumpingRulePriority;
    let i = 0;
    let j = 0;
    for (let room of this.bumpingRooms) {
      if (this.index !== i) {
        room.BumpingRulePriority = j++;
      }
      i++;
    }
    this.setAsDirty();
  }
  bump() {
    if (!this.room.BumpingRulePriority) {
      this.room.BumpingRulePriority = this.bumpingRooms.length;
      this.setAsDirty();
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
    this.setAsDirty();
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
    this.setAsDirty();
  }

  setAsDirty() {
    this.setDirtyChild.emit();
  }

  isTypeVacancy() {
    return this.selectedBumpingType === Room.BUMPING_TYPE.VACANCY;
  }

}
