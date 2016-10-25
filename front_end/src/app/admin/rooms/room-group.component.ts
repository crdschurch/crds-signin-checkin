import { Component, Input } from '@angular/core';
import { AdminService } from '../admin.service';
import { Range } from './range';
import { Group } from './group';
import * as _ from 'lodash'

@Component({
  selector: '.group',
  templateUrl: 'room-group.component.html',
  styleUrls: ['room-group.component.scss'],
  providers: [ AdminService ]
})
export class RoomGroupComponent {
  @Input() group: Group;

  constructor( private adminService: AdminService) {
  }

  toggleAll(group: Group) {
    const newState = !group.Selected
    group.Selected = newState;
    for (let range of group.Ranges) {
        range.Selected = newState;
    }
  }

  toggleRange(range: Range, group: Group) {
    const newState = !range.Selected
    range.Selected = newState;
    console.log(newState)
    if (!newState) {
      // if turning off, set group selected to false
      group.Selected = false;
    } else {
      // if turning on, if all are turned on select the All button
      const allSelected = _.every(group.Ranges, ['Selected', true]);
      console.log(group.Ranges, allSelected)
      if (allSelected) group.Selected = true;
    }
  }

}
