import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Room } from '../../../shared/models';
import { AdminService } from '../../admin.service';

@Component({
  selector: '.room-bump',
  templateUrl: 'room-bump.component.html',
  styleUrls: ['room-bump.component.scss']
})
export class RoomBumpComponent implements OnInit {
  @Input() room: Room;
  @Input() bumpingRooms: Room[];
  @Input() allRooms: Room[];
  @Input() index: number;

  constructor( private adminService: AdminService,
               private route: ActivatedRoute) {}

  ngOnInit() {}

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
    this.updateBumpingRooms();
  }
  bump() {
    if (!this.room.BumpingRulePriority) {
      this.room.BumpingRulePriority = this.bumpingRooms.length;
      this.updateBumpingRooms();
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
    this.updateBumpingRooms();
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
    this.updateBumpingRooms();
  }

  private updateBumpingRooms() {
    this.adminService.updateBumpingRooms(this.room.EventId, this.route.snapshot.params['roomId'], this.allRooms);
  }

}
