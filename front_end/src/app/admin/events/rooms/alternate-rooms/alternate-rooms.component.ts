import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Room } from '../../../../shared/models';
import { AdminService } from '../../../admin.service';

@Component({
  selector: 'alternate-rooms',
  templateUrl: 'alternate-rooms.component.html',
  styleUrls: ['alternate-rooms.component.scss']
})
export class AlternateRoomsComponent implements OnInit {
  private _allRooms: Room[];

  constructor( private adminService: AdminService,
               private route: ActivatedRoute) {}

  ngOnInit() {
    this.adminService.getBumpingRooms(this.route.snapshot.params['eventId'], this.route.snapshot.params['roomId']).subscribe(
      allRooms => {
        this.allRooms = Room.fromJsons(allRooms);
      },
      error => {
        console.error(error);
      }
    );
  }

  get allRooms() {
    return this._allRooms;
  }

  set allRooms(rooms) {
    this._allRooms = rooms;
  }

  get availableRooms(): Room[] {
    if (this._allRooms) {
      return this._allRooms.filter( (obj: Room) => { return !obj.isBumpingRoom(); } );
    };
  }

  get bumpingRooms(): Room[] {
    if (this._allRooms) {
      return this._allRooms
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