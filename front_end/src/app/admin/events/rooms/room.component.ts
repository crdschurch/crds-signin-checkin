import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { AdminService } from '../../admin.service';
import { Room } from '../../../shared/models';

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

  constructor(private route: ActivatedRoute, private adminService: AdminService) {
  }

  mainEventId() {
    return this.route.snapshot.params['eventId'];
  }

  add(field) {
    this.roomForm.controls[field].setValue(this.room[field]++);
  }
  remove(field) {
    if (this.room[field] >= 1) {
      this.roomForm.controls[field].setValue(this.room[field]--);
    }
  }
  toggle(field) {
    this.room[field] = !this.room[field];
    this.roomForm.controls[field].setValue(this.room[field]);
  }

  sync(field) {
    this.room[field] = this.roomForm.controls[field].value;
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
    return this.room.EventId != this.mainEventId();
  }

  ageRangeAndGrades(): any {
    let ageGrades = this.room.getSelectionDescription(false);

    if (ageGrades.length === 0) {
      ageGrades = ['Add'];
    }

    return ageGrades;
  }

  ngOnInit() {

    this.roomForm = new FormGroup({
      Volunteers: new FormControl(),
      Capacity: new FormControl(),
      AllowSignIn: new FormControl()
    });

    this.roomForm.valueChanges
      .debounceTime(1000)
      .distinctUntilChanged()
      .subscribe(props => {
        this.pending = true;
        this.adminService.updateRoom(this.room.EventId, this.room.RoomId, this.room).subscribe(room => {
          this.room = room;
          this.pending = false;
        });
      });
  }
}
