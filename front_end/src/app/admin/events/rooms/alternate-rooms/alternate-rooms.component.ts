import { Component, Output, Input, EventEmitter } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Room } from '../../../../shared/models';
import { AdminService } from '../../../admin.service';

@Component({
  selector: 'alternate-rooms',
  templateUrl: 'alternate-rooms.component.html',
  styleUrls: ['alternate-rooms.component.scss']
})
export class AlternateRoomsComponent {
  @Input() allAlternateRooms: Room[];
  @Output() setDirty = new EventEmitter<boolean>();

  constructor( private adminService: AdminService,
               private route: ActivatedRoute) {}

  setDirtyChild() {
    this.setDirty.emit(true);
  }

  get allRooms() {
    return this.allAlternateRooms;
  }

  set allRooms(rooms) {
    this.allAlternateRooms = rooms;
  }

  get availableRooms(): Room[] {
    if (this.allAlternateRooms) {
      return this.allAlternateRooms.filter( (obj: Room) => { return !obj.isBumpingRoom(); } );
    };
  }

  get bumpingRooms(): Room[] {
    if (this.allAlternateRooms) {
      return this.allAlternateRooms
        .filter(
          (obj: Room) => { return obj.isBumpingRoom(); })
        .sort(
          (a, b) => {
            if (a.BumpingRulePriority > b.BumpingRulePriority) {
              return 1;
            } else if (a.BumpingRulePriority < b.BumpingRulePriority) {
              return -1;
            } else {
              return 0;
            }
          }
        );
    }
  }

}
