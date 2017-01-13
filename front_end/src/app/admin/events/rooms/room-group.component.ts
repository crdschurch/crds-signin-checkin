import { Component, Input, Output, EventEmitter } from '@angular/core';
import { AdminService } from '../../admin.service';
import { Range, Group, Room, Event } from '../../../shared/models';
import * as _ from 'lodash';

@Component({
  selector: '.group',
  templateUrl: 'room-group.component.html',
  styleUrls: ['room-group.component.scss']
})
export class RoomGroupComponent {
  @Input() group: Group;
  @Input() eventToUpdate: Event;
  @Input() roomId: string;
  @Input() room: Room;

  // my stuff
  @Output() setDirty = new EventEmitter<boolean>();

  constructor( private adminService: AdminService) {
  }

  toggleAll(group: Group) {
    const newState = !group.Selected;
    group.Selected = newState;
    if (group.Ranges) {
      for (let range of group.Ranges) {
          range.Selected = newState;
      }
    }

    this.setDirty.emit(true);
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

    this.setDirty.emit(true);
  }

}
