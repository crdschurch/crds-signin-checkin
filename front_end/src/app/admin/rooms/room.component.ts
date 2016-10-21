import { Component, Input, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { FormControl, FormGroup } from '@angular/forms';
import { AdminService } from '../admin.service';
import { Room } from './room';

@Component({
  selector: '.room',
  templateUrl: 'room.component.html',
  styleUrls: ['room.component.scss']
})
export class RoomComponent implements OnInit {
  @Input() room: Room;

  private roomForm: FormGroup;

  constructor( private adminService: AdminService) {
  }

  add(field): void {
    this.roomForm.controls[field].setValue(this.room[field]++);
  }
  remove(field): void {
    if(this.room[field] >= 1) {
      this.roomForm.controls[field].setValue(this.room[field]--);
    }
  }
  toggle(field): void {
    this.room[field] = !this.room[field]
    this.roomForm.controls[field].setValue(this.room[field]);
  }

  ngOnInit() {
    this.roomForm = new FormGroup({
      Volunteers: new FormControl(),
      Capacity: new FormControl(),
      AllowSignIn: new FormControl()
    });

    this.roomForm.valueChanges
      .debounceTime(1000)
      .subscribe(props => {
        this.adminService.updateRoom(this.room.EventId, this.room.RoomId, this.room).subscribe(room => {
          this.room = room;
        })
      });
  }
}
