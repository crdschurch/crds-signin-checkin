import { Component, Input } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { AdminService } from '../admin.service';
import { Room } from './room';

@Component({
  selector: '.room',
  templateUrl: 'room.component.html',
  styleUrls: ['room.component.scss']
})
export class RoomComponent {
  @Input() room: Room;

  roomForm = new FormGroup({
    Volunteers: new FormControl(),
    Capacity: new FormControl(),
    AllowSignIn: new FormControl()
  });

  constructor( private adminService: AdminService) {
    this.roomForm.valueChanges
      .debounceTime(1000)
      .subscribe(props => {
        this.adminService.updateRoom(this.room.EventId, this.room.RoomId, this.room).subscribe(val => {})
      });
  }

  add(field): void {
    this.roomForm.controls[field].setValue(this.room[field]++);
  }
  remove(field): void {
    this.roomForm.controls[field].setValue(this.room[field]--);
  }
  toggle(field): void {
    this.room[field] = !this.room[field]
    this.roomForm.controls[field].setValue(this.room[field]);
  }
}
