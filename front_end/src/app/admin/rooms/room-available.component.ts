// import { Component, Input } from '@angular/core';
// import { AdminService } from '../admin.service';
// import { Range } from './range';
// import { Group } from './group';
// import { Room } from './room';
// import * as _ from 'lodash';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Room } from './room';

@Component({
  selector: '.available',
  templateUrl: 'room-available.component.html',
  styleUrls: ['room-available.component.scss']
})
export class RoomAvailableComponent {

  @Input() room: Room;
  @Output() roomSelected = new EventEmitter<string>();

  constructor() {
  }

  selectRoom() {
    // call up to the parent, pop one item from one list to the other
    this.roomSelected.emit(this.room.RoomId);
  }

  // toggleAll(group: Group) {
  //   const newState = !group.Selected;
  //   group.Selected = newState;
  //   if (group.Ranges) {
  //     for (let range of group.Ranges) {
  //         range.Selected = newState;
  //     }
  //   }
  //   this.adminService.updateRoomGroups(this.eventId, this.roomId, this.room);
  // }

  // toggleRange(range: Range, group: Group) {
  //   const newState = !range.Selected;
  //   range.Selected = newState;
  //   if (!newState) {
  //     // if turning off, set group selected to false
  //     group.Selected = false;
  //   } else {
  //     // if turning on, if all are turned on select the All button
  //     const allSelected = _.every(group.Ranges, ['Selected', true]);
  //     if (allSelected) {
  //       group.Selected = true;
  //     }
  //   }
  //   this.adminService.updateRoomGroups(this.eventId, this.roomId, this.room);
  // }

}
