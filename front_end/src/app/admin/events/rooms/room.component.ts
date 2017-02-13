import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { AdminService } from '../../admin.service';
import { RootService } from '../../../shared/services';
import { Room } from '../../../shared/models';
import * as _ from 'lodash';

@Component({
  selector: '.room',
  templateUrl: 'room.component.html',
  styleUrls: ['room.component.scss'],
  providers: [ ]
})
export class RoomComponent implements OnInit {
  @Input() room: Room;
  public pending: boolean;
  private roomForm: FormGroup;
  private origRoomData: Room;
  public changed: boolean;

  constructor(private route: ActivatedRoute, private adminService: AdminService, private rootService: RootService) {
  }

  mainEventId() {
    return this.route.snapshot.params['eventId'];
  }

  highlight(e) {
    e.target.select();
  }

  add(field) {
    this.room[field]++;
    this.change();
  }

  remove(field) {
    if (this.room[field] >= 1) {
      this.room[field]--
      this.change();
    }
  }
  toggle(field) {
    this.room[field] = !this.room[field];
    this.change();
  }

  saveRoom() {
    this.pending = true;
    this.adminService.updateRoom(this.room.EventId, this.room.RoomId, this.room).subscribe(room => {
      this.origRoomData = _.clone(this.room);
      this.room = room;
      this.changed = false;
      this.pending = false;
    }, (error) => {
      this.room = this.origRoomData;
      this.changed = false;
      this.pending = false;
      this.rootService.announceEvent('generalError');
    });
    return false;
  }

  hasCapacity() {
    return this.room.Capacity;
  }

  checkedInEqualsCapacity() {
    return this.room.CheckedIn >= this.room.Capacity;
  }

  signedInWillEqualCapacity() {
    // only return true if checkedInEqualsCapacity isnt true
    if (!this.checkedInEqualsCapacity()) {
      return this.room.SignedIn + this.room.CheckedIn >= this.room.Capacity;
    }
  }

  getRoomRatioString() {
    if (this.room.CheckedIn || this.room.Volunteers) {
      return `${this.room.CheckedIn}/${this.room.Volunteers}`;
    } else {
      return '0';
    }
  }

  toggleClick() {
    if (this.pending) {
      return false;
    }
  }

  isAdventureClub() {
    return Number(this.room.EventId) !== Number(this.mainEventId());
  }

  ageRangeAndGrades(): any {
    let ageGrades = this.room.getSelectionDescription(false);

    if (ageGrades.length === 0) {
      ageGrades = ['Add'];
    }

    return ageGrades;
  }

  change() {
    this.changed = true;
  }

  ngOnInit() {
    this.origRoomData = _.clone(this.room);
  }
}
