import { Component, Input, EventEmitter } from '@angular/core';
import { AdminService } from '../admin.service';
import { Range } from './range';
import { Group } from './group';
import { Room } from './room';
import { Observable } from 'rxjs/Rx';
import * as _ from 'lodash';

@Component({
  selector: '.group',
  templateUrl: 'room-group.component.html',
  styleUrls: ['room-group.component.scss'],
  providers: [ AdminService ]
})
export class RoomGroupComponent {
  @Input() group: Group;
  @Input() eventId: string;
  @Input() roomId: string;
  @Input() room: Room;
  updateEmitter: EventEmitter<Room> = new EventEmitter<Room>();
  updateObserver: Observable<Room>;

  constructor( private adminService: AdminService) {
    this.updateObserver = this.updateEmitter.map(room => room).debounceTime(2000);
    this.updateObserver.subscribe(room => {
      this.adminService.updateRoomGroups(room.EventId, room.RoomId, room);
    });
  }

  toggleAll(group: Group) {
    const newState = !group.Selected;
    group.Selected = newState;
    if (group.Ranges) {
      for (let range of group.Ranges) {
          range.Selected = newState;
      }
    }
    this.updateEmitter.emit(this.room);
    // this.adminService.updateRoomGroups(this.eventId, this.roomId, this.room).subscribe(room => {});
  }

  toggleRange(range: Range, group: Group) {
    const newState = !range.Selected;
    range.Selected = newState;
    if (!newState) {
      // if turning off, set group selected to false
      group.Selected = false;
    } else {
      // if turning on, if all are turned on select the All button
      const allSelected = _.every(group.Ranges, ['Selected', true]);
      if (allSelected) {
        group.Selected = true;
      }
    }
    this.updateEmitter.emit(this.room);
    // this.adminService.updateRoomGroups(this.eventId, this.roomId, this.room).subscribe(room => {});
  }

}
