import { Component, Input, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { AdminService } from '../admin.service';
import { Room } from './room';

@Component({
  selector: '.room',
  templateUrl: 'room.component.html',
  styleUrls: ['room.component.scss'],
  providers: [ AdminService ]
})
export class RoomComponent implements OnInit {
  @Input() room: Room;
  public pending: boolean;
  private roomForm: FormGroup;

  constructor( private adminService: AdminService) {
  }

  add(field) {
    this.roomForm.controls[field].setValue(this.room[field]++);
  }
  remove(field) {
    if(this.room[field] >= 1) {
      this.roomForm.controls[field].setValue(this.room[field]--);
    }
  }
  toggle(field) {
    this.room[field] = !this.room[field]
    this.roomForm.controls[field].setValue(this.room[field]);
  }

  toggleClick() {
    if(this.pending) {
      return false;
    }
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
        })
      });
  }
}
