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
  private updateEmitter: EventEmitter<Room>;
  private updateObserver: Observable<Room>;

  constructor( private adminService: AdminService) {
    // Create an emitter to use when sending updates to the room
    this.updateEmitter = new EventEmitter<Room>();

    // Setup an observer on the emitter, and set it to debounce for 2 seconds.
    // This prevents the frontend from sending a backend update if multiple
    // age ranges or grades are selected quickly.
    this.updateObserver =
      this.updateEmitter.map(room => room).debounceTime(2000);

    // Subscribe to the debounced event - now actually send the update to
    // the backend.
    // TODO - Should handle the response, and notify the user of success or failure
    // TODO - Should have some sort of processing state while the update is running, since it can take several seconds to complete 
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
  }

}
