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
  // volunteers = new FormControl();

  coolForm = new FormGroup({
    volunteers: new FormControl(),
    capacity: new FormControl()
  });

  constructor(
    private adminService: AdminService) {
     this.coolForm.valueChanges
        .debounceTime(1000)
        .subscribe(props => {
          console.log("c", this.room)
          this.adminService.updateRoom(this.room.EventId, this.room.RoomId, this.room).subscribe(val => {
            console.log("val", val);
          })
        });
    }

  ngOnInit(): void {
    console.log("room component oninit", this)
  }

  addVolunteer(): void { this.coolForm.controls["volunteers"].setValue(this.room.Volunteers++); }
  removeVolunteer(): void { this.coolForm.controls["volunteers"].setValue(this.room.Volunteers--); }
  addCapacity(): void { this.coolForm.controls["capacity"].setValue(this.room.Capacity++); }
  removeCapacity(): void { this.coolForm.controls["capacity"].setValue(this.room.Capacity--); }
}
